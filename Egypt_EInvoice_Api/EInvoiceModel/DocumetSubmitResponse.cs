using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class DocumetSubmitResponse
    {
        public string submissionUUID { get; set; }
        public List<AcceptedDocument> acceptedDocuments { get; set; }
        public List<RejectedDocument> rejectedDocuments { get; set; }
    }
}
