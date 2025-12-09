using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class Notification
    {
        public string notificationId { get; set; }
        public DateTime receivedDateTime { get; set; }
        public DateTime deliveredDateTime { get; set; }
        public string typeId { get; set; }
        public string typeName { get; set; }
        public string finalMessage { get; set; }
        public string channel { get; set; }
        public string address { get; set; }
        public string language { get; set; }
        public string status { get; set; }
        public List<DeliveryAttempt> deliveryAttempts { get; set; }
    }
}
