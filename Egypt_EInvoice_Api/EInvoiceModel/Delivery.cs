using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class Delivery
    {
        public string approach { get; set; }
        public string packaging { get; set; }
        public string dateValidity { get; set; }
        public string exportPort { get; set; }
        public string countryOfOrigin { get; set; } //Optional: Country of origin of goods/services. Country represented by ISO-3166-2 2 symbol code of the countries.
        public decimal? grossWeight { get; set; }
        public decimal? netWeight { get; set; }
        public string terms { get; set; }
    }
}
