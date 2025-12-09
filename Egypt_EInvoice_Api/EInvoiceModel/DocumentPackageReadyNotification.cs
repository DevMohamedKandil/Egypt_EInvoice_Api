using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class DocumentPackageReadyNotification
    {
        public string deliveryId { get; set; }
        public string type { get; set; }
        public int count { get; set; }
        public List<DocumentPackageReadyMessage> message { get; set; }
    }
}
