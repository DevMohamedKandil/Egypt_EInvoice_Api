using System;

namespace Egypt_EInvoice_Api.Models
{
    public class VWEInvoice
    {
        public string InternalId { get; set; }
        public DateTime DateTimeIssued { get; set; }

        public string IssuerName { get; set; }
        public string IssuerId { get; set; }

        public double? TotalSalesAmount { get; set; }
        public double? ExtraDiscountAmount { get; set; }
        public double? TotalItemsDiscountAmount { get; set; }
        public double? TotalDiscountAmount { get; set; }
        public double? NetAmount { get; set; }
        public double? TotalAmount { get; set; }

        public double? AddTax { get; set; }
    }
}