using Egypt_EInvoice_Api;
using Microsoft.Extensions.Logging;
using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ess;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;

public class TokenSigner
{
    private static readonly byte[] Sha256DigestInfoPrefix =
    {
        0x30, 0x31, 0x30, 0x0d, 0x06, 0x09, 0x60, 0x86,
        0x48, 0x01, 0x65, 0x03, 0x04, 0x02, 0x01, 0x05,
        0x00, 0x04, 0x20
    };

    private static readonly string[] AutoSelectionKeywords =
    {
        "egypt trust",
        "egypttrust",
        "signing",
        "signature",
        "seal",
        "stamp",
        "invoice",
        "e tax",
        "etax",
        "e invoice",
        "einvoice"
    };

    private readonly EtaSigningOptions options;
    private readonly ILogger logger;

    public string SelectedCertificateThumbprint { get; private set; }
    public string SelectedCertificateSerialNumber { get; private set; }
    public string SelectedCertificateLabel { get; private set; }
    public string SelectedCertificateId { get; private set; }

    public TokenSigner(EtaSigningOptions options, ILogger logger = null)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.logger = logger;
    }

    public string SignWithCMS(string serializedJson)
    {
        ValidateOptions();

        byte[] data = Encoding.UTF8.GetBytes(serializedJson);
        byte[] canonicalContentHash = HashBytes(data);

        Pkcs11InteropFactories factories = new Pkcs11InteropFactories();
        using (IPkcs11Library pkcs11Library = factories.Pkcs11LibraryFactory.LoadPkcs11Library(
            factories,
            options.Pkcs11LibraryPath,
            AppType.MultiThreaded))
        {
            ISlot slot = pkcs11Library.GetSlotList(SlotsType.WithTokenPresent).FirstOrDefault();
            if (slot == null)
                throw new InvalidOperationException("No Egypt Trust USB token is connected or visible to the PKCS#11 library.");

            ITokenInfo tokenInfo = slot.GetTokenInfo();
            logger?.LogInformation(
                "ETA token slot selected. SlotId: {SlotId}, TokenLabel: {TokenLabel}, TokenSerial: {TokenSerial}, TokenModel: {TokenModel}",
                tokenInfo.SlotId,
                tokenInfo.Label?.Trim(),
                tokenInfo.SerialNumber?.Trim(),
                tokenInfo.Model?.Trim());

            using (ISession session = slot.OpenSession(SessionType.ReadWrite))
            {
                Login(session);

                try
                {
                    TokenCertificate tokenCertificate = FindTokenCertificate(session);
                    IObjectHandle privateKey = tokenCertificate.PrivateKey;
                    if (privateKey == null)
                        throw new InvalidOperationException("The selected signing certificate does not have a usable matching RSA private key on the USB token.");

                    X509Certificate2 certificate = tokenCertificate.Certificate;
                    SelectedCertificateThumbprint = certificate.Thumbprint;
                    SelectedCertificateSerialNumber = certificate.SerialNumber;
                    SelectedCertificateLabel = tokenCertificate.Label;
                    SelectedCertificateId = tokenCertificate.IdHex;

                    logger?.LogInformation(
                        "ETA token certificate selected. Reason: {SelectionReason}, Label: {CertificateLabel}, Id: {CertificateId}, Subject: {CertificateSubject}, Issuer: {CertificateIssuer}, Serial: {CertificateSerialNumber}, Thumbprint: {CertificateThumbprint}",
                        tokenCertificate.SelectionReason,
                        SelectedCertificateLabel,
                        SelectedCertificateId,
                        certificate.Subject,
                        certificate.Issuer,
                        SelectedCertificateSerialNumber,
                        SelectedCertificateThumbprint);

                    logger?.LogInformation(
                        "Using token certificate: Label={CertificateLabel}, Serial={CertificateSerialNumber}, Id={CertificateId}, Subject={CertificateSubject}",
                        SelectedCertificateLabel,
                        SelectedCertificateSerialNumber,
                        SelectedCertificateId,
                        certificate.Subject);

                    logger?.LogInformation("ETA CMS signing started with token private key. CertificateId: {CertificateId}", SelectedCertificateId);

                    RSAParameters publicParameters;
                    using (RSA publicKey = certificate.GetRSAPublicKey())
                    {
                        publicParameters = publicKey.ExportParameters(false);
                    }

                    using (Pkcs11Rsa rsa = new Pkcs11Rsa(session, privateKey, publicParameters))
                    {
                        // Do not call X509Certificate2.CopyWithPrivateKey here. That API can
                        // force private key parameter export, which hardware USB tokens must
                        // reject. The RSA bridge below signs hashes through PKCS#11 directly.
                        logger?.LogInformation("Using SignedCms with non-exportable token RSA bridge");

                        ContentInfo content = new ContentInfo(new Oid("1.2.840.113549.1.7.5"), data);
                        SignedCms cms = new SignedCms(content, true);

                        EssCertIDv2 bouncyCertificate = new EssCertIDv2(
                            new Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier(
                                new DerObjectIdentifier("2.16.840.1.101.3.4.2.1")),
                            HashBytes(certificate.RawData));

                        SigningCertificateV2 signerCertificateV2 =
                            new SigningCertificateV2(new[] { bouncyCertificate });

                        CmsSigner signer = new CmsSigner(SubjectIdentifierType.IssuerAndSerialNumber, certificate, rsa)
                        {
                            DigestAlgorithm = new Oid("2.16.840.1.101.3.4.2.1"),
                            IncludeOption = X509IncludeOption.EndCertOnly
                        };

                        signer.SignedAttributes.Add(new Pkcs9SigningTime(DateTime.UtcNow));
                        signer.SignedAttributes.Add(new AsnEncodedData(
                            new Oid("1.2.840.113549.1.9.16.2.47"),
                            signerCertificateV2.GetEncoded()));

                        cms.ComputeSignature(signer);
                        VerifyCmsMessageDigest(cms, canonicalContentHash);

                        string signature = Convert.ToBase64String(cms.Encode());

                        logger?.LogInformation(
                            "ETA CMS signing completed with token private key. CertificateId: {CertificateId}, CanonicalSha256: {CanonicalSha256}, SignatureBase64Length: {SignatureBase64Length}",
                            SelectedCertificateId,
                            ToHex(canonicalContentHash),
                            signature.Length);

                        return signature;
                    }
                }
                finally
                {
                    try
                    {
                        session.Logout();
                    }
                    catch
                    {
                    }
                }
            }
        }
    }

    public string Serialize(JObject request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        StringBuilder serialized = new StringBuilder();
        SerializeToken(request, serialized, null);
        return serialized.ToString();
    }

    private void ValidateOptions()
    {
        if (string.IsNullOrWhiteSpace(options.Pkcs11LibraryPath))
            throw new InvalidOperationException("ETA signing PKCS#11 library path is not configured.");
        if (string.IsNullOrWhiteSpace(options.TokenPin))
            throw new InvalidOperationException("ETA signing token PIN is not configured.");
    }

    private bool HasCertificateSelector()
    {
        return !string.IsNullOrWhiteSpace(options.CertificateSerialNumber)
            || !string.IsNullOrWhiteSpace(options.CertificateLabel)
            || !string.IsNullOrWhiteSpace(options.CertificateSubjectContains)
            || !string.IsNullOrWhiteSpace(options.CertificateId);
    }

    private void Login(ISession session)
    {
        try
        {
            session.Login(CKU.CKU_USER, Encoding.UTF8.GetBytes(options.TokenPin));
        }
        catch (Pkcs11Exception ex) when (ex.RV == CKR.CKR_USER_ALREADY_LOGGED_IN)
        {
            logger?.LogDebug("PKCS#11 user was already logged in to the token session.");
        }
        catch (Pkcs11Exception ex) when (ex.RV == CKR.CKR_PIN_INCORRECT)
        {
            throw new InvalidOperationException("USB token PIN is incorrect.", ex);
        }
        catch (Pkcs11Exception ex) when (ex.RV == CKR.CKR_PIN_LOCKED)
        {
            throw new InvalidOperationException("USB token PIN is locked.", ex);
        }
    }

    private TokenCertificate FindTokenCertificate(ISession session)
    {
        List<TokenCertificate> candidates = DiscoverTokenCertificates(session);

        logger?.LogInformation(
            "ETA token certificate selector values. CertificateId: {CertificateId}, CertificateSerialNumber: {CertificateSerialNumber}, CertificateLabel: {CertificateLabel}, CertificateSubjectContains: {CertificateSubjectContains}",
            options.CertificateId,
            options.CertificateSerialNumber,
            options.CertificateLabel,
            options.CertificateSubjectContains);

        TokenCertificate selected = SelectByConfiguredSelectors(candidates);
        if (selected != null)
            return selected;

        selected = SelectAutomatically(candidates);
        if (selected != null)
            return selected;

        throw new InvalidOperationException(BuildNoCertificateSelectedMessage(candidates));
    }

    private List<TokenCertificate> DiscoverTokenCertificates(ISession session)
    {
        var certificateSearchAttributes = new List<IObjectAttribute>
        {
            session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CLASS, CKO.CKO_CERTIFICATE),
            session.Factories.ObjectAttributeFactory.Create(CKA.CKA_TOKEN, true),
            session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CERTIFICATE_TYPE, CKC.CKC_X_509)
        };

        List<TokenCertificate> candidates = new List<TokenCertificate>();
        foreach (IObjectHandle certificateHandle in session.FindAllObjects(certificateSearchAttributes))
        {
            TokenCertificate candidate = ReadTokenCertificate(session, certificateHandle);
            PopulatePrivateKeyMetadata(session, candidate);
            candidates.Add(candidate);

            logger?.LogInformation(
                "ETA token certificate discovered. Label: {CertificateLabel}, Id: {CertificateId}, Subject: {CertificateSubject}, Issuer: {CertificateIssuer}, Serial: {CertificateSerialNumber}, Thumbprint: {CertificateThumbprint}, HasPrivateKey: {HasPrivateKey}, PrivateKeyLabel: {PrivateKeyLabel}, PrivateKeyId: {PrivateKeyId}",
                candidate.Label,
                candidate.IdHex,
                candidate.Certificate.Subject,
                candidate.Certificate.Issuer,
                candidate.Certificate.SerialNumber,
                candidate.Certificate.Thumbprint,
                candidate.HasMatchingPrivateKey,
                candidate.PrivateKeyLabel,
                candidate.PrivateKeyId);
        }

        if (candidates.Count == 0)
            logger?.LogWarning("No X.509 certificate objects were discovered on the connected USB token.");

        return candidates;
    }

    private TokenCertificate ReadTokenCertificate(ISession session, IObjectHandle certificateHandle)
    {
        byte[] rawCertificate = GetAttributeBytes(session, certificateHandle, CKA.CKA_VALUE);
        if (rawCertificate == null || rawCertificate.Length == 0)
            throw new InvalidOperationException("A certificate object was found on the USB token, but its CKA_VALUE could not be read.");

        return new TokenCertificate
        {
            Handle = certificateHandle,
            Certificate = new X509Certificate2(rawCertificate),
            Id = GetAttributeBytes(session, certificateHandle, CKA.CKA_ID),
            Label = GetAttributeString(session, certificateHandle, CKA.CKA_LABEL)
        };
    }

    private void PopulatePrivateKeyMetadata(ISession session, TokenCertificate tokenCertificate)
    {
        try
        {
            IObjectHandle privateKey;
            string privateKeyLabel;
            string privateKeyId;
            int matchingKeyCount;

            if (!TryFindMatchingPrivateKey(session, tokenCertificate, out privateKey, out privateKeyLabel, out privateKeyId, out matchingKeyCount))
                return;

            tokenCertificate.PrivateKey = privateKey;
            tokenCertificate.PrivateKeyLabel = privateKeyLabel;
            tokenCertificate.PrivateKeyId = privateKeyId;
            tokenCertificate.MatchingPrivateKeyCount = matchingKeyCount;

            logger?.LogInformation(
                "ETA token matching private key found. CertificateId: {CertificateId}, CertificateLabel: {CertificateLabel}, PrivateKeyLabel: {PrivateKeyLabel}, PrivateKeyId: {PrivateKeyId}, MatchingKeyCount: {MatchingKeyCount}",
                tokenCertificate.IdHex,
                tokenCertificate.Label,
                privateKeyLabel,
                privateKeyId,
                matchingKeyCount);
        }
        catch (Exception ex)
        {
            logger?.LogWarning(
                ex,
                "Failed while checking for a matching private key for token certificate. Label: {CertificateLabel}, Id: {CertificateId}, Subject: {CertificateSubject}",
                tokenCertificate.Label,
                tokenCertificate.IdHex,
                tokenCertificate.Certificate.Subject);
        }
    }

    private bool TryFindMatchingPrivateKey(
        ISession session,
        TokenCertificate tokenCertificate,
        out IObjectHandle privateKey,
        out string privateKeyLabel,
        out string privateKeyId,
        out int matchingKeyCount)
    {
        privateKey = null;
        privateKeyLabel = null;
        privateKeyId = null;
        matchingKeyCount = 0;

        List<IObjectHandle> matchingKeys = new List<IObjectHandle>();

        if (tokenCertificate.Id != null && tokenCertificate.Id.Length > 0)
        {
            matchingKeys = FindPrivateKeys(
                session,
                session.Factories.ObjectAttributeFactory.Create(CKA.CKA_ID, tokenCertificate.Id));
        }

        if (matchingKeys.Count == 0 && !string.IsNullOrWhiteSpace(tokenCertificate.Label))
        {
            matchingKeys = FindPrivateKeys(
                session,
                session.Factories.ObjectAttributeFactory.Create(CKA.CKA_LABEL, tokenCertificate.Label));
        }

        if (matchingKeys.Count == 0)
            return false;

        privateKey = matchingKeys.First();
        privateKeyLabel = GetAttributeString(session, privateKey, CKA.CKA_LABEL);
        privateKeyId = ToHex(GetAttributeBytes(session, privateKey, CKA.CKA_ID));
        matchingKeyCount = matchingKeys.Count;

        return true;
    }

    private List<IObjectHandle> FindPrivateKeys(ISession session, IObjectAttribute extraAttribute)
    {
        List<IObjectAttribute> privateKeySearchAttributes = new List<IObjectAttribute>
        {
            session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CLASS, CKO.CKO_PRIVATE_KEY),
            session.Factories.ObjectAttributeFactory.Create(CKA.CKA_TOKEN, true),
            session.Factories.ObjectAttributeFactory.Create(CKA.CKA_KEY_TYPE, CKK.CKK_RSA)
        };

        if (extraAttribute != null)
            privateKeySearchAttributes.Add(extraAttribute);

        return session.FindAllObjects(privateKeySearchAttributes).ToList();
    }

    private TokenCertificate SelectByConfiguredSelectors(List<TokenCertificate> candidates)
    {
        foreach (CertificateSelectorAttempt attempt in BuildSelectorAttempts())
        {
            List<TokenCertificate> matches = candidates
                .Where(attempt.Matches)
                .OrderBy(CertificateSortKey)
                .ToList();

            List<TokenCertificate> usableMatches = matches
                .Where(candidate => candidate.HasMatchingPrivateKey)
                .OrderBy(CertificateSortKey)
                .ToList();

            logger?.LogInformation(
                "ETA token certificate selector attempted. Selector: {SelectorName}, Value: {SelectorValue}, MatchCount: {MatchCount}, UsableMatchCount: {UsableMatchCount}",
                attempt.Name,
                attempt.Value,
                matches.Count,
                usableMatches.Count);

            if (usableMatches.Count == 0)
                continue;

            if (usableMatches.Count > 1)
            {
                logger?.LogWarning(
                    "Multiple usable token certificates matched selector {SelectorName}; selecting the first deterministic candidate. Candidates: {Candidates}",
                    attempt.Name,
                    FormatCandidates(usableMatches));
            }

            TokenCertificate selected = usableMatches.First();
            selected.SelectionReason = "matched configured " + attempt.Name + " selector";
            return selected;
        }

        if (!HasCertificateSelector())
            logger?.LogInformation("No ETA token certificate selector is configured; automatic token certificate selection will be used.");
        else
            logger?.LogWarning(
                "No usable ETA token certificate matched the configured selector values. Attempted selectors: {SelectorValues}. Automatic token certificate selection will be used.",
                BuildConfiguredSelectorsSummary());

        return null;
    }

    private TokenCertificate SelectAutomatically(List<TokenCertificate> candidates)
    {
        foreach (TokenCertificate candidate in candidates)
        {
            candidate.AutoSelectionScore = CalculateAutoSelectionScore(candidate);
        }

        List<TokenCertificate> usableCandidates = candidates
            .Where(candidate => candidate.HasMatchingPrivateKey)
            .OrderByDescending(candidate => candidate.AutoSelectionScore)
            .ThenBy(CertificateSortKey)
            .ToList();

        if (usableCandidates.Count == 0)
            return null;

        int topScore = usableCandidates.First().AutoSelectionScore;
        List<TokenCertificate> topCandidates = usableCandidates
            .Where(candidate => candidate.AutoSelectionScore == topScore)
            .OrderBy(CertificateSortKey)
            .ToList();

        if (topCandidates.Count > 1)
        {
            logger?.LogWarning(
                "Multiple usable token certificates remained after automatic scoring; selecting the first deterministic candidate. Candidates: {Candidates}",
                FormatCandidates(topCandidates));
        }

        TokenCertificate selected = topCandidates.First();
        selected.SelectionReason = "automatically selected by token heuristics; score " + topScore.ToString(CultureInfo.InvariantCulture);

        logger?.LogInformation(
            "ETA token certificate automatically selected. Reason: {SelectionReason}, CandidateScores: {CandidateScores}",
            selected.SelectionReason,
            FormatScoredCandidates(usableCandidates));

        return selected;
    }

    private List<CertificateSelectorAttempt> BuildSelectorAttempts()
    {
        List<CertificateSelectorAttempt> attempts = new List<CertificateSelectorAttempt>();

        if (!string.IsNullOrWhiteSpace(options.CertificateId))
        {
            string configuredId = options.CertificateId;
            attempts.Add(new CertificateSelectorAttempt
            {
                Name = "CertificateId",
                Value = configuredId,
                Matches = candidate => NormalizedHexEquals(candidate.IdHex, configuredId)
            });
        }

        if (!string.IsNullOrWhiteSpace(options.CertificateSerialNumber))
        {
            string configuredSerial = options.CertificateSerialNumber;
            attempts.Add(new CertificateSelectorAttempt
            {
                Name = "CertificateSerialNumber",
                Value = configuredSerial,
                Matches = candidate => SerialMatches(candidate.Certificate.SerialNumber, configuredSerial)
            });
        }

        if (!string.IsNullOrWhiteSpace(options.CertificateLabel))
        {
            string configuredLabel = options.CertificateLabel;
            attempts.Add(new CertificateSelectorAttempt
            {
                Name = "CertificateLabel",
                Value = configuredLabel,
                Matches = candidate => NormalizedTextEquals(candidate.Label, configuredLabel)
            });
        }

        if (!string.IsNullOrWhiteSpace(options.CertificateSubjectContains))
        {
            string configuredSubject = options.CertificateSubjectContains;
            attempts.Add(new CertificateSelectorAttempt
            {
                Name = "CertificateSubjectContains",
                Value = configuredSubject,
                Matches = candidate => NormalizedTextContains(candidate.Certificate.Subject, configuredSubject)
            });
        }

        return attempts;
    }

    private int CalculateAutoSelectionScore(TokenCertificate candidate)
    {
        int score = 0;

        if (candidate.HasMatchingPrivateKey)
            score += 1000;

        if (!IsCertificateAuthority(candidate.Certificate))
            score += 100;
        else
            score -= 100;

        if (HasSigningKeyUsage(candidate.Certificate))
            score += 75;

        string searchableText = ((candidate.Label ?? string.Empty) + " " + (candidate.Certificate.Subject ?? string.Empty)).ToLowerInvariant();
        foreach (string keyword in AutoSelectionKeywords)
        {
            if (searchableText.Contains(keyword))
                score += 25;
        }

        return score;
    }

    private static bool IsCertificateAuthority(X509Certificate2 certificate)
    {
        X509BasicConstraintsExtension extension = certificate.Extensions
            .OfType<X509BasicConstraintsExtension>()
            .FirstOrDefault();

        return extension != null && extension.CertificateAuthority;
    }

    private static bool HasSigningKeyUsage(X509Certificate2 certificate)
    {
        X509KeyUsageExtension extension = certificate.Extensions
            .OfType<X509KeyUsageExtension>()
            .FirstOrDefault();

        if (extension == null)
            return true;

        return extension.KeyUsages.HasFlag(X509KeyUsageFlags.DigitalSignature)
            || extension.KeyUsages.HasFlag(X509KeyUsageFlags.NonRepudiation);
    }

    private static string CertificateSortKey(TokenCertificate candidate)
    {
        return string.Join(
            "|",
            NormalizeText(candidate.Label),
            NormalizeHex(candidate.IdHex),
            NormalizeHex(candidate.Certificate.SerialNumber),
            NormalizeHex(candidate.Certificate.Thumbprint));
    }

    private string BuildNoCertificateSelectedMessage(List<TokenCertificate> candidates)
    {
        if (candidates == null || candidates.Count == 0)
            return "No X.509 signing certificates were found on the connected USB token. Confirm the Egypt Trust token is connected, unlocked, and visible to the configured PKCS#11 library.";

        return "No usable ETA signing certificate could be selected from the USB token. "
            + "Configured selectors attempted: " + BuildConfiguredSelectorsSummary() + ". "
            + "Discovered token certificates: " + FormatCandidates(candidates)
            + ". Configure one of CertificateId, CertificateSerialNumber, CertificateLabel, or CertificateSubjectContains using the exact discovered values, or leave selectors empty for automatic selection.";
    }

    private string BuildConfiguredSelectorsSummary()
    {
        List<string> parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(options.CertificateId))
            parts.Add("CertificateId=" + options.CertificateId);
        if (!string.IsNullOrWhiteSpace(options.CertificateSerialNumber))
            parts.Add("CertificateSerialNumber=" + options.CertificateSerialNumber);
        if (!string.IsNullOrWhiteSpace(options.CertificateLabel))
            parts.Add("CertificateLabel=" + options.CertificateLabel);
        if (!string.IsNullOrWhiteSpace(options.CertificateSubjectContains))
            parts.Add("CertificateSubjectContains=" + options.CertificateSubjectContains);

        return parts.Count == 0 ? "none" : string.Join("; ", parts);
    }

    private static string FormatCandidates(IEnumerable<TokenCertificate> candidates)
    {
        return string.Join(" | ", candidates.Select(candidate =>
            "Label=" + SafeValue(candidate.Label)
            + ", Id=" + SafeValue(candidate.IdHex)
            + ", Subject=" + SafeValue(candidate.Certificate.Subject)
            + ", Issuer=" + SafeValue(candidate.Certificate.Issuer)
            + ", Serial=" + SafeValue(candidate.Certificate.SerialNumber)
            + ", Thumbprint=" + SafeValue(candidate.Certificate.Thumbprint)
            + ", HasPrivateKey=" + candidate.HasMatchingPrivateKey
            + ", PrivateKeyLabel=" + SafeValue(candidate.PrivateKeyLabel)
            + ", PrivateKeyId=" + SafeValue(candidate.PrivateKeyId)
            + ", IsCA=" + IsCertificateAuthority(candidate.Certificate)));
    }

    private static string FormatScoredCandidates(IEnumerable<TokenCertificate> candidates)
    {
        return string.Join(" | ", candidates.Select(candidate =>
            "Score=" + candidate.AutoSelectionScore.ToString(CultureInfo.InvariantCulture)
            + ", Label=" + SafeValue(candidate.Label)
            + ", Id=" + SafeValue(candidate.IdHex)
            + ", Serial=" + SafeValue(candidate.Certificate.SerialNumber)
            + ", Subject=" + SafeValue(candidate.Certificate.Subject)));
    }

    private static byte[] GetAttributeBytes(ISession session, IObjectHandle handle, CKA attributeType)
    {
        try
        {
            IObjectAttribute attribute = session.GetAttributeValue(handle, new List<CKA> { attributeType }).FirstOrDefault();
            if (attribute == null || attribute.CannotBeRead)
                return null;

            return attribute.GetValueAsByteArray();
        }
        catch
        {
            return null;
        }
    }

    private static string GetAttributeString(ISession session, IObjectHandle handle, CKA attributeType)
    {
        try
        {
            IObjectAttribute attribute = session.GetAttributeValue(handle, new List<CKA> { attributeType }).FirstOrDefault();
            if (attribute == null || attribute.CannotBeRead)
                return null;

            return attribute.GetValueAsString()?.Trim();
        }
        catch
        {
            return null;
        }
    }

    private static string NormalizeHex(string value)
    {
        return (value ?? string.Empty)
            .Replace(" ", string.Empty)
            .Replace(":", string.Empty)
            .Replace("-", string.Empty)
            .ToUpperInvariant();
    }

    private static bool NormalizedHexEquals(string first, string second)
    {
        string normalizedFirst = NormalizeHex(first);
        string normalizedSecond = NormalizeHex(second);

        return normalizedFirst.Length > 0
            && normalizedSecond.Length > 0
            && string.Equals(normalizedFirst, normalizedSecond, StringComparison.OrdinalIgnoreCase);
    }

    private static bool SerialMatches(string tokenSerial, string configuredSerial)
    {
        string normalizedTokenSerial = NormalizeHex(tokenSerial);
        string normalizedConfiguredSerial = NormalizeHex(configuredSerial);

        if (normalizedTokenSerial.Length == 0 || normalizedConfiguredSerial.Length == 0)
            return false;

        if (string.Equals(normalizedTokenSerial, normalizedConfiguredSerial, StringComparison.OrdinalIgnoreCase))
            return true;

        string reversedTokenSerial = ReverseHexBytes(normalizedTokenSerial);
        string reversedConfiguredSerial = ReverseHexBytes(normalizedConfiguredSerial);

        return string.Equals(reversedTokenSerial, normalizedConfiguredSerial, StringComparison.OrdinalIgnoreCase)
            || string.Equals(normalizedTokenSerial, reversedConfiguredSerial, StringComparison.OrdinalIgnoreCase);
    }

    private static string ReverseHexBytes(string normalizedHex)
    {
        if (string.IsNullOrWhiteSpace(normalizedHex))
            return string.Empty;

        if (normalizedHex.Length % 2 != 0)
            normalizedHex = "0" + normalizedHex;

        StringBuilder reversed = new StringBuilder(normalizedHex.Length);
        for (int i = normalizedHex.Length - 2; i >= 0; i -= 2)
        {
            reversed.Append(normalizedHex.Substring(i, 2));
        }

        return reversed.ToString();
    }

    private static string NormalizeText(string value)
    {
        return (value ?? string.Empty).Trim().ToUpperInvariant();
    }

    private static bool NormalizedTextEquals(string first, string second)
    {
        string normalizedFirst = NormalizeText(first);
        string normalizedSecond = NormalizeText(second);

        return normalizedFirst.Length > 0
            && normalizedSecond.Length > 0
            && string.Equals(normalizedFirst, normalizedSecond, StringComparison.OrdinalIgnoreCase);
    }

    private static bool NormalizedTextContains(string value, string contains)
    {
        string normalizedValue = NormalizeText(value);
        string normalizedContains = NormalizeText(contains);

        return normalizedValue.Length > 0
            && normalizedContains.Length > 0
            && normalizedValue.Contains(normalizedContains);
    }

    private static string SafeValue(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? "<empty>" : value.Trim();
    }

    private static string ToHex(byte[] value)
    {
        if (value == null || value.Length == 0)
            return string.Empty;

        return BitConverter.ToString(value).Replace("-", string.Empty);
    }

    private static byte[] HashBytes(byte[] input)
    {
        using (SHA256 sha = SHA256.Create())
        {
            return sha.ComputeHash(input);
        }
    }

    private static byte[] BuildDigestInfo(byte[] hash, HashAlgorithmName hashAlgorithm)
    {
        if (hashAlgorithm != HashAlgorithmName.SHA256)
            throw new CryptographicException("Only SHA-256 is supported for ETA token signing.");

        byte[] digestInfo = new byte[Sha256DigestInfoPrefix.Length + hash.Length];
        Buffer.BlockCopy(Sha256DigestInfoPrefix, 0, digestInfo, 0, Sha256DigestInfoPrefix.Length);
        Buffer.BlockCopy(hash, 0, digestInfo, Sha256DigestInfoPrefix.Length, hash.Length);
        return digestInfo;
    }

    private static void SerializeToken(JToken token, StringBuilder serialized, string arrayPropertyName)
    {
        if (token == null || token.Type == JTokenType.Null)
            return;

        if (token.Type == JTokenType.Object)
        {
            foreach (JProperty property in token.Children<JProperty>())
            {
                SerializeProperty(property, serialized);
            }

            return;
        }

        if (token.Type == JTokenType.Array)
        {
            if (!string.IsNullOrWhiteSpace(arrayPropertyName))
                serialized.Append('"').Append(arrayPropertyName.ToUpperInvariant()).Append('"');

            foreach (JToken item in token.Children())
            {
                if (!string.IsNullOrWhiteSpace(arrayPropertyName))
                    serialized.Append('"').Append(arrayPropertyName.ToUpperInvariant()).Append('"');

                SerializeToken(item, serialized, null);
            }

            return;
        }

        SerializeValue(token, serialized);
    }

    private static void SerializeProperty(JProperty property, StringBuilder serialized)
    {
        string propertyName = property.Name.ToUpperInvariant();

        if (property.Value.Type == JTokenType.Null)
            return;

        if (property.Value.Type == JTokenType.Array)
        {
            SerializeToken(property.Value, serialized, propertyName);
            return;
        }

        serialized.Append('"').Append(propertyName).Append('"');
        SerializeToken(property.Value, serialized, null);
    }

    private static void SerializeValue(JToken value, StringBuilder serialized)
    {
        switch (value.Type)
        {
            case JTokenType.Integer:
            case JTokenType.Float:
                serialized.Append('"')
                    .Append(Convert.ToString(((JValue)value).Value, CultureInfo.InvariantCulture))
                    .Append('"');
                break;
            case JTokenType.Boolean:
                serialized.Append('"')
                    .Append(value.Value<bool>() ? "true" : "false")
                    .Append('"');
                break;
            case JTokenType.Date:
                throw new InvalidOperationException("Date token found while canonicalizing ETA invoice JSON. Parse invoice JSON with DateParseHandling.None before signing so the signed content exactly matches the submitted JSON.");
            default:
                serialized.Append(JsonConvert.ToString(value.Value<string>()));
                break;
        }
    }

    private void VerifyCmsMessageDigest(SignedCms cms, byte[] expectedDigest)
    {
        byte[] messageDigest = GetCmsMessageDigest(cms);
        if (messageDigest == null || messageDigest.Length == 0)
            throw new CryptographicException("CMS messageDigest signed attribute was not found after signing.");

        if (!messageDigest.SequenceEqual(expectedDigest))
        {
            throw new CryptographicException(
                "CMS messageDigest signed attribute does not match SHA-256 of the canonical ETA content. Expected "
                + ToHex(expectedDigest)
                + " but CMS contains "
                + ToHex(messageDigest));
        }

        logger?.LogInformation(
            "CMS messageDigest verified against canonical ETA content. CanonicalSha256: {CanonicalSha256}",
            ToHex(expectedDigest));
    }

    private static byte[] GetCmsMessageDigest(SignedCms cms)
    {
        if (cms == null || cms.SignerInfos.Count == 0)
            return null;

        foreach (CryptographicAttributeObject attribute in cms.SignerInfos[0].SignedAttributes)
        {
            if (attribute?.Oid?.Value != "1.2.840.113549.1.9.4" || attribute.Values == null || attribute.Values.Count == 0)
                continue;

            return ReadDerOctetString(attribute.Values[0].RawData);
        }

        return null;
    }

    private static byte[] ReadDerOctetString(byte[] rawData)
    {
        if (rawData == null || rawData.Length == 0)
            return null;

        if (rawData.Length == 32)
            return rawData;

        if (rawData[0] != 0x04 || rawData.Length < 2)
            return rawData;

        int offset = 1;
        int length = rawData[offset++];

        if ((length & 0x80) != 0)
        {
            int lengthByteCount = length & 0x7F;
            if (lengthByteCount == 0 || lengthByteCount > 4 || offset + lengthByteCount > rawData.Length)
                return rawData;

            length = 0;
            for (int i = 0; i < lengthByteCount; i++)
                length = (length << 8) | rawData[offset++];
        }

        if (length < 0 || offset + length > rawData.Length)
            return rawData;

        byte[] value = new byte[length];
        Buffer.BlockCopy(rawData, offset, value, 0, length);
        return value;
    }

    private class TokenCertificate
    {
        public IObjectHandle Handle { get; set; }
        public X509Certificate2 Certificate { get; set; }
        public byte[] Id { get; set; }
        public string Label { get; set; }
        public IObjectHandle PrivateKey { get; set; }
        public string PrivateKeyLabel { get; set; }
        public string PrivateKeyId { get; set; }
        public int MatchingPrivateKeyCount { get; set; }
        public int AutoSelectionScore { get; set; }
        public string SelectionReason { get; set; }
        public string IdHex => ToHex(Id);
        public bool HasMatchingPrivateKey => PrivateKey != null;
    }

    private class CertificateSelectorAttempt
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public Func<TokenCertificate, bool> Matches { get; set; }
    }

    private class Pkcs11Rsa : RSA
    {
        private readonly ISession session;
        private readonly IObjectHandle privateKey;
        private readonly RSAParameters publicParameters;
        private readonly object signLock = new object();

        public Pkcs11Rsa(ISession session, IObjectHandle privateKey, RSAParameters publicParameters)
        {
            this.session = session;
            this.privateKey = privateKey;
            this.publicParameters = publicParameters;
            KeySizeValue = publicParameters.Modulus.Length * 8;
            LegalKeySizesValue = new[] { new KeySizes(1024, 4096, 1024) };
        }

        public override string KeyExchangeAlgorithm => "RSA";
        public override string SignatureAlgorithm => "RSA";

        public override byte[] Decrypt(byte[] data, RSAEncryptionPadding padding)
        {
            throw new NotSupportedException("The ETA token RSA bridge only supports signing.");
        }

        public override byte[] Encrypt(byte[] data, RSAEncryptionPadding padding)
        {
            throw new NotSupportedException("The ETA token RSA bridge only supports signing.");
        }

        public override RSAParameters ExportParameters(bool includePrivateParameters)
        {
            if (includePrivateParameters)
                throw new NotSupportedException("Private key parameters cannot be exported from the USB token.");

            return publicParameters;
        }

        public override void ImportParameters(RSAParameters parameters)
        {
            throw new NotSupportedException("Importing parameters is not supported by the ETA token RSA bridge.");
        }

        public override byte[] SignHash(byte[] hash, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
        {
            if (padding != RSASignaturePadding.Pkcs1)
                throw new CryptographicException("Only RSA PKCS#1 v1.5 padding is supported for ETA token signing.");

            byte[] digestInfo = BuildDigestInfo(hash, hashAlgorithm);
            IMechanism mechanism = session.Factories.MechanismFactory.Create(CKM.CKM_RSA_PKCS);

            lock (signLock)
            {
                return session.Sign(mechanism, privateKey, digestInfo);
            }
        }

        public override bool VerifyHash(byte[] hash, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(publicParameters);
                return rsa.VerifyHash(hash, signature, hashAlgorithm, padding);
            }
        }
    }
}
