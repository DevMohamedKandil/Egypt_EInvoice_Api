using System;

namespace Egypt_EInvoice_Api.Models
{
    // =========================================================
    // VwEInvoiceMasterDto
    // =========================================================
    public class VwEInvoiceMasterDto
    {
        public string InternalId { get; set; }
        public Guid? Guid { get; set; }
        public DateTime? Date { get; set; }
        public string BillNo { get; set; }
        public string IssuerName { get; set; }
        public string IssuerId { get; set; }
        public double? TotalSalesAmount { get; set; }
        public double? TotalDiscountAmount { get; set; }
        public double? NetAmount { get; set; }
        public double? TotalAmount { get; set; }
        public double? AddTax { get; set; }
        public double? ExtraDiscountAmount { get; set; }
        public double? TotalItemsDiscountAmount { get; set; }
        public bool? IsUploaded { get; set; }
        public Guid? TypeGuid { get; set; }
        public string IssuerType { get; set; }
        public string EInvoiceGuid { get; set; }
    }

    // =========================================================
    // VWEInvoiceDto
    // =========================================================
    public class VWEInvoiceDto
    {
        public string InternalId { get; set; }
        public DateTime DateTimeIssued { get; set; }
        public string IssuerName { get; set; }
        public string IssuerId { get; set; }
        public double? TotalSalesAmount { get; set; }
        public double? TotalDiscountAmount { get; set; }
        public double? NetAmount { get; set; }
        public double? TotalAmount { get; set; }
    }

    // =========================================================
    // VWInvoiceLineDto — double to match VWInvoiceLine (SQL float columns)
    // =========================================================
    public class VWInvoiceLineDto
    {
        public Guid Guid { get; set; }
        public Guid BillGuid { get; set; }
        public Guid MatGuid { get; set; }
        public string description { get; set; }
        public string itemType { get; set; }
        public string itemCode { get; set; }
        public double quantity { get; set; }
        public string currencySold { get; set; }
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
    }
}