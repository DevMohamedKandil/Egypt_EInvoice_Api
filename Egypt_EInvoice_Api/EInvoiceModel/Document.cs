using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class Document
    {
        public Issuer issuer { get; set; }
        public Receiver receiver { get; set; }
        public string documentType { get; set; } //Document type name.( Must be i for invoice. === Must be c for credit note. === Must be d for debit note.)
        public string documentTypeVersion { get; set; } //Document type version name. Must be 1.0 for this version.
        public string dateTimeIssued { get; set; } //The date and time when the document was issued. Date and time cannot be in future. Time to be supplied in UTC timezone.
        public string taxpayerActivityCode { get; set; } //Tax activity code of the business issuing the document representing the activity that caused it to be issued. Must be valid activity type code.
        public string internalID { get; set; } // Internal document ID used to link back to the ERP document number. Value defined by the submitter.
        public string purchaseOrderReference { get; set; }
        public string purchaseOrderDescription { get; set; }
        public string salesOrderReference { get; set; }
        public string salesOrderDescription { get; set; }
        public string proformaInvoiceNumber { get; set; }
        public Payment payment { get; set; }
        public Delivery delivery { get; set; }
        public InvoiceLine[] invoiceLines { get; set; } // اصناف الفاتورة

        public decimal totalSalesAmount { get; set; } //Sum all all InvoiceLine/SalesTotal items
        public decimal totalDiscountAmount { get; set; } //Total amount of discounts: sum of all Discount amount elements of InvoiceLine items.
        public decimal netAmount { get; set; } //TotalSales – TotalDiscount
        //public List<TaxTotal> taxTotals { get; set; }//cancelled by mohamed fawzy

        public TaxTotal[] taxTotals { get; set; }//added by mohamed fawzy

        public decimal extraDiscountAmount { get; set; } //خصم اضافي على اجمالي الفاتورة  Additional discount amount applied at the level of the overall document, not individual lines.
        public decimal totalItemsDiscountAmount { get; set; }  //Total amount of item discounts: sum of all Item Discount amount elements of InvoiceLine items.
        public decimal totalAmount { get; set; } // Total amount of the invoice calculated as NetAmount + Totals of tax amounts. 5 decimal digits allowed
        
        
    //    public Signature[] signatures { get; set; }

     //   public string[] references { get; set; } // CreditNote - references



    }
}
