using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class NotificationResponse
    {
        public List<Notification> result { get; set; }
        public List<DocumentMetadata> metadata { get; set; }
    }
}
