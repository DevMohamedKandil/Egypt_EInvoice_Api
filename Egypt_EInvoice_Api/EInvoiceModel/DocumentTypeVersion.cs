using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class DocumentTypeVersion
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public decimal versionNumber { get; set; }
        public string status { get; set; }
        public DateTime activeFrom { get; set; }
        public DateTime? activeTo { get; set; }

        public string jsonSchema { get; set; }
        public string xmlSchema { get; set; }
    }
}
