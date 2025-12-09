using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class SubmissionDocument
    {
        public string uuid { get; set; }
        public int documentCount { get; set; }
        public DateTime dateTimeReceived { get; set; }
        public string overallStatus { get; set; } //Overall status of the batch processing. Values: in progress, valid, partially valid, invalid
        public List<DocumentSummary> documentSummary { get; set; }
        public DocumentMetadata documentSummaryMetadata { get; set; }
    }
}
