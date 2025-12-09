using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class Payment
    {
        public string bankName { get; set; }
        public string bankAddress { get; set; }
        public string bankAccountNo { get; set; }
        public string bankAccountIBAN { get; set; }
        public string swiftCode { get; set; }
        public string terms { get; set; }
    }
}
