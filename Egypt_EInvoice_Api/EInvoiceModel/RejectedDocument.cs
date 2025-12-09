using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class RejectedDocument
    {
        public string internalId { get; set; }
        public Error error { get; set; }
    }
}
