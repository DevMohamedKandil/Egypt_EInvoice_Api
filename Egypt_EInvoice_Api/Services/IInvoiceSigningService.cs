using Newtonsoft.Json.Linq;

namespace Egypt_EInvoice_Api.Services
{
    public interface IInvoiceSigningService
    {
        SignedInvoiceDocument SignDocument(JObject unsignedDocument, string billNo, string internalId);
    }
}
