using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class Address
    {
        public string branchId { get; set; } //Mandatory when issuer is of type B, otherwise optional. The code of the branch as registered with tax authority for the company submitting the document.
        public string country { get; set; }
        public string governate { get; set; }
        public string regionCity { get; set; }
        public string street { get; set; }
        public string buildingNumber { get; set; }
        public string postalCode { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string landmark { get; set; }

        public string additionalInformation { get; set; }

    }
}
