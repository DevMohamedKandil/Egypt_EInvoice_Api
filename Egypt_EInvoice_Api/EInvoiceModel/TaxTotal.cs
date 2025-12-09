using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class TaxTotal
    {
        public string taxType { get; set; }
        public decimal amount { get; set; } //Sum of all amounts of given tax in all invoice lines. 5 decimal digits allowed.
    }
}
