using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Egypt_EInvoice_Api.Models
{
    public class VWInvoiceLine
    {
        [Key]
        public Guid Guid { get; set; }
        public Guid BillGuid { get; set; }
        public Guid MatGuid { get; set; }
        public string description { get; set; }
        public string itemType { get; set; } //Coding schema used to encode the item type. Must be GS1 or EGS for this version.
        public string itemCode { get; set; } //Code of the goods or services item being sold. GS1 codes targeted for managing goods, EGS codes targeted for managing goods – goods or services.
        //public string unitType { get; set; } //Code of the unit type used from the code table of the measures.
        public double quantity { get; set; }
        //=======================unitValue===========================
        public string currencySold { get; set; }
        public double amountEGP { get; set; }
        public double amountSold { get; set; }
        public double currencyExchangeRate { get; set; }
        //============================================================
        public double salesTotal { get; set; } //السعر في الكمية Total amount for the invoice line considering quantity and unit price in EGP (with excluded factory amounts if they are present for specific types in documents).
        public double total { get; set; }  //Total amount for the invoice line after adding all pricing items, taxes, removing discounts
        public decimal valueDifference { get; set; } //Value difference when selling goods already taxed (accepts +/- numbers), e.g., factory value based.
        public double totalTaxableFees { get; set; } //Total amount of additional taxable fees to be used in final tax calculation. اجمالي الرسوم الاضافية الخاضعة للضريبة
        public double netTotal { get; set; } // Total amount for the invoice line after applying discount.
        public double itemsDiscount { get; set; } //Non-taxable items discount.
        //===============Discount======================================
        public double discRate { get; set; }
        public double discAmount { get; set; }
        //=============================================================
        public string internalCode { get; set; }

        public double? AddTax { get; set; }//added by mohamed fawzy

        public double? TaxPercent { get; set; }//added by mohamed fawzy

    }
}
