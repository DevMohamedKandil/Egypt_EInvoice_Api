using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class DeliveryAttempt
    {
        public DateTime attemptDateTime { get; set; }
        public string status { get; set; }
        public string statusDetails { get; set; }
    }
}
