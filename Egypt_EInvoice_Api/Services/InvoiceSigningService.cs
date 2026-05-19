using Egypt_EInvoice_Api.EInvoiceModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Egypt_EInvoice_Api.Services
{
    public class InvoiceSigningService : IInvoiceSigningService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<InvoiceSigningService> logger;

        public InvoiceSigningService(IConfiguration configuration, ILogger<InvoiceSigningService> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public SignedInvoiceDocument SignDocument(JObject unsignedDocument, string billNo, string internalId)
        {
            Appsettings settings = configuration.GetRequiredSection("Settings").Get<Appsettings>();
            if (settings.Signing == null)
                throw new InvalidOperationException("ETA signing configuration section is missing.");

            logger.LogInformation(
                "ETA signing started. BillNo: {BillNo}, InternalId: {InternalId}, ConfiguredSerial: {CertificateSerialNumber}, ConfiguredLabel: {CertificateLabel}, ConfiguredSubject: {CertificateSubjectContains}, ConfiguredCertificateId: {CertificateId}",
                billNo,
                internalId,
                settings.Signing.CertificateSerialNumber,
                settings.Signing.CertificateLabel,
                settings.Signing.CertificateSubjectContains,
                settings.Signing.CertificateId);

            TokenSigner tokenSigner = new TokenSigner(settings.Signing, logger);
            string canonicalContent = tokenSigner.Serialize(unsignedDocument);
            string canonicalContentSha256 = ComputeSha256Hex(canonicalContent);
            string unsignedJson = unsignedDocument.ToString(Formatting.None);

            logger.LogInformation(
                "ETA canonical content prepared. BillNo: {BillNo}, InternalId: {InternalId}, CanonicalLength: {CanonicalLength}, CanonicalSha256: {CanonicalSha256}, UnsignedJson: {UnsignedJson}, CanonicalContent: {CanonicalContent}",
                billNo,
                internalId,
                canonicalContent.Length,
                canonicalContentSha256,
                unsignedJson,
                canonicalContent);

            string signature = tokenSigner.SignWithCMS(canonicalContent);

            JObject signedDocument = (JObject)unsignedDocument.DeepClone();
            signedDocument["signatures"] = JArray.FromObject(new[]
            {
                new Signature
                {
                    signatureType = "I",
                    value = signature
                }
            });

            string signedJson = signedDocument.ToString(Formatting.None);

            logger.LogInformation(
                "ETA signing completed. BillNo: {BillNo}, InternalId: {InternalId}, SelectedThumbprint: {CertificateThumbprint}, SelectedSerial: {CertificateSerialNumber}, SelectedLabel: {CertificateLabel}, SelectedCertificateId: {CertificateId}, CanonicalSha256: {CanonicalSha256}, SignatureBase64Length: {SignatureBase64Length}, SignedJson: {SignedJson}",
                billNo,
                internalId,
                tokenSigner.SelectedCertificateThumbprint,
                tokenSigner.SelectedCertificateSerialNumber,
                tokenSigner.SelectedCertificateLabel,
                tokenSigner.SelectedCertificateId,
                canonicalContentSha256,
                signature.Length,
                signedJson);

            return new SignedInvoiceDocument
            {
                SignedDocument = signedDocument,
                Signature = signature,
                CertificateThumbprint = tokenSigner.SelectedCertificateThumbprint,
                CertificateSerialNumber = tokenSigner.SelectedCertificateSerialNumber,
                CertificateLabel = tokenSigner.SelectedCertificateLabel,
                CertificateId = tokenSigner.SelectedCertificateId,
                CanonicalContent = canonicalContent,
                CanonicalContentSha256 = canonicalContentSha256
            };
        }

        private static string ComputeSha256Hex(string value)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }
    }
}
