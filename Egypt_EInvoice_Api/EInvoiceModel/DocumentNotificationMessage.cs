using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class DocumentNotificationMessage
    {
        public string type { get; set; } //Notification type being delivered. For this notification batch can be document-validated, document-received, document-rejected, document-cancelled
        public string uuid { get; set; }
        public string submissionUUID { get; set; }
        public string longId { get; set; }
        public string internalId { get; set; }
        public string status { get; set; }
    }
}
