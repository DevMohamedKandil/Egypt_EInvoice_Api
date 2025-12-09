using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class Issuer
    {
        public string type { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public Address address { get; set; }
    }
}
