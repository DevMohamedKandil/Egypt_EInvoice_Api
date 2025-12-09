using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class Discount
    {
        public decimal rate { get; set; } //Optional: discount percentage rate applied. Must be from 0 to 100.
        public decimal amount { get; set; }
    }
}
