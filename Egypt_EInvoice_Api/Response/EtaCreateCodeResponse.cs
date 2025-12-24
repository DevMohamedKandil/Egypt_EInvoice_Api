using System.Net;

namespace Egypt_EInvoice_Api.Response
{
    public class EtaCreateCodeResponse
    {
        public bool IsSuccess { get; set; }
        public string RawResponse { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
