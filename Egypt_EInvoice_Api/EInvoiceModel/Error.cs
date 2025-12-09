using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class Error
    {
        public string code { get; set; }
        public string message { get; set; }
        public string target { get; set; }
        public List<Error> details { get; set; }
    }
}
