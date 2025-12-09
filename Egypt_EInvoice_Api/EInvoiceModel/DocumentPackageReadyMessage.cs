using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class DocumentPackageReadyMessage
    {
        public string type { get; set; } //Notification type being delivered. For this notification batch can be document-package-ready
        public string packageId { get; set; }
    }
}
