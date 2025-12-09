using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class DocumentDetail
    {
        public string submissionUUID { get; set; }
        public string longId { get; set; }
        public DateTime dateTimeReceived { get; set; }
        public string status { get; set; }

        public string transformationStatus { get; set; }
        public DocumentValidationResult validationResults { get; set; }
        public Document document { get; set; }
        public DateTime cancelRequestDate { get; set; }
        public DateTime rejectRequestDate { get; set; }
        public DateTime cancelRequestDelayedDate { get; set; }
        public DateTime rejectRequestDelayedDate { get; set; }
        public DateTime declineCancelRequestDate { get; set; }
        public DateTime declineRejectRequestDate { get; set; }
    }
}
