using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Egypt_EInvoice_Api.Models
{
    public class VWInvoiceLine
    {
        [Key]
        public Guid Guid { get; set; }
        public Guid BillGuid { get; set; }
        public Guid MatGuid { get; set; }
        public string description { get; set; }
        public string itemType { get; set; }
        public string itemCode { get; set; }

        public string currencySold { get; set; }

        // SQL float → C# double
        public double amountEGP { get; set; }
        public double amountSold { get; set; }
        public double currencyExchangeRate { get; set; }
        public double salesTotal { get; set; }
        public double total { get; set; }
        public double valueDifference { get; set; }
        public double totalTaxableFees { get; set; }
        public double netTotal { get; set; }
        public double itemsDiscount { get; set; }
        public double discRate { get; set; }
        public double discAmount { get; set; }
        public string internalCode { get; set; }
        public double? AddTax { get; set; }
        public double? TaxPercent { get; set; }
        public double quantity { get; set; }
        public string unitType { get; set; }
    }
}