using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class TaxableItem
    {
        public string taxType { get; set; } //	Type of tax applied - from the list of approved tax type codes. The TaxType needs to be unique across the invoice line (no VAT twice in one invoice line), TaxType is from the list of supported tax types.
        public decimal amount { get; set; }
        public string subType { get; set; }
        public decimal rate { get; set; }
    }
}
