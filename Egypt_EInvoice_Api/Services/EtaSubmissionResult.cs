using Egypt_EInvoice_Api.EInvoiceModel;
using System.Net;

namespace Egypt_EInvoice_Api.Services
{
    public class EtaSubmissionResult
    {
        public HttpStatusCode StatusCode { get; set; }
        public string RawResponse { get; set; }
        public DocumetSubmitResponse Response { get; set; }
        public bool IsDuplicatePayload { get; set; }
        public string ErrorMessage { get; set; }
    }
}
