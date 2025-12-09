using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class Receiver
    {
        public string type { get; set; } //B for Bussiness, P for person, F for foreigner
        public string id { get; set; } //Registration number. For business in Egypt must be registration number. For residents must be national ID. For foreign buyers must be VAT ID of the foreign company. Optional if person buyer and invoice amount less than threshold limit defined. Receiver and issuer cannot be the same.
        public string name { get; set; }
        public Address address { get; set; }

    }
}
