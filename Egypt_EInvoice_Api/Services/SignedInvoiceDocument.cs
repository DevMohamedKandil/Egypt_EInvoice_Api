using Newtonsoft.Json.Linq;

namespace Egypt_EInvoice_Api.Services
{
    public class SignedInvoiceDocument
    {
        public JObject SignedDocument { get; set; }
        public string Signature { get; set; }
        public string CertificateThumbprint { get; set; }
        public string CertificateSerialNumber { get; set; }
        public string CertificateLabel { get; set; }
        public string CertificateId { get; set; }
        public string CanonicalContent { get; set; }
        public string CanonicalContentSha256 { get; set; }
    }
}
