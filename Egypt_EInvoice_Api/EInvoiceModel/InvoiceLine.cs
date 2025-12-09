using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class InvoiceLine

    {
        //Invoice Details
        public string description { get; set; }
        public string itemType { get; set; } //Coding schema used to encode the item type. Must be GS1 or EGS for this version.
        public string itemCode { get; set; } //Code of the goods or services item being sold. GS1 codes targeted for managing goods, EGS codes targeted for managing goods – goods or services.
        public string unitType { get; set; } //Code of the unit type used from the code table of the measures.
        public decimal quantity { get; set; }
        public UnitValue unitValue { get; set; }
        public decimal salesTotal { get; set; } //السعر في الكمية Total amount for the invoice line considering quantity and unit price in EGP (with excluded factory amounts if they are present for specific types in documents).
        public decimal total { get; set; }  //Total amount for the invoice line after adding all pricing items, taxes, removing discounts
        public decimal valueDifference { get; set; } //Value difference when selling goods already taxed (accepts +/- numbers), e.g., factory value based.
        public decimal totalTaxableFees { get; set; } //Total amount of additional taxable fees to be used in final tax calculation. اجمالي الرسوم الاضافية الخاضعة للضريبة
        public decimal netTotal { get; set; } // Total amount for the invoice line after applying discount.
        public decimal itemsDiscount { get; set; } //Non-taxable items discount.
        public Discount discount { get; set; }
        // public List<TaxableItem> taxableItems { get; set; }//uncanceled by mohamed fawzy
        public TaxableItem[] taxableItems { get; set; }//added by mohamed fawzy

        public string internalCode { get; set; }


    }
}
