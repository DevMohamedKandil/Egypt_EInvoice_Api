using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Egypt_EInvoice_Api.Models
{
    public class Bill
    {

        [Key]
        public Guid Guid { get; set; }
        public Guid TypeGuid { get; set; }
        public int? Type { get; set; }
        public int Number { get; set; }
        public string BillNo { get; set; }
        public Guid? CustGuid { get; set; }

        public int? CustNumber { get; set; }

        public string CustName { get; set; }

        public string Phone { get; set; }


        public string Fax { get; set; }
        public string Address { get; set; }

        public DateTime? Date { get; set; }


        public int? CurrencyPtr { get; set; }
        public double? CurrencyVal { get; set; }
        public string Notes { get; set; }
        public double? Total { get; set; }
        public double? Paid { get; set; }
        public double? Remain { get; set; }
        public int? AccPaidNumber { get; set; }
        public Guid? AccPaidGuid { get; set; }
        public int? AccRemianNumber { get; set; }
        public Guid? AccRemainGuid { get; set; }

        public int? CEntry { get; set; }

        public int? PayType { get; set; }

        public double? Period { get; set; }
        public double? TotalDiscValue { get; set; }
        public int? AccDiscValue { get; set; }
        public string NotesDiscValue { get; set; }
        public double? TotalDiscPer { get; set; }
        public int? AccDiscPer { get; set; }
        public string NotesDiscPer { get; set; }
        public double? TotalExtra { get; set; }
        public int? AccExtra { get; set; }
        public string NotesExtra { get; set; }
        public int IsPosted { get; set; }
        public int? SalesManPtr { get; set; }
        public int? CostPtr { get; set; }
        public string TransOrderPtr { get; set; }

        public int? AccBillNumber { get; set; }

        public int? StoreNumber { get; set; }
        public Guid? AccBillGuid { get; set; }
        public Guid? StoreGUID { get; set; }
        public Guid? TransOrderGUID { get; set; }
        public Guid? PaymentGuid { get; set; }
        public Guid? DelegateGuid { get; set; }
        public Guid? ClientGUID { get; set; }
        public double FinalTotal { get; set; }
        public int OrderTotalDisc { get; set; }
        public int OrderTotalPerDisc { get; set; }
        public int OrderTotalExtra { get; set; }
        public int? UserNumber { get; set; }
        public Guid? CostGuid { get; set; }
        public double? CashPaid { get; set; }
        public Guid? PeriodGuid { get; set; }
        public Guid? CashierGuid { get; set; }
        public double? TotalPaid { get; set; }
        public string Reference { get; set; }
        public string RefNumber { get; set; }
        public DateTime? RefDate { get; set; }
        public Guid? RefGUID { get; set; }
        public DateTime? ActDate { get; set; }
        public string? CompName { get; set; }
        public int? Branch { get; set; }
        public Guid? EnGuid { get; set; }
        public Guid? ReserveGuid { get; set; }
        public Guid? RefTGUID { get; set; }
        public int? Flag { get; set; }
        public Guid? CarGuid { get; set; }
        public Guid? TrailerGuid { get; set; }
        public string Code { get; set; }
        public Guid? CheckTypeGuid { get; set; }
        public Guid? PrStepGuid { get; set; }
        public Guid? DriverGuid { get; set; }
        public string Field1 { get; set; }

        public string Field2 { get; set; }

        public string Field3 { get; set; }
        public string Field4 { get; set; }
        public double? BillExtra { get; set; }
        public Guid? TranBillGuid { get; set; }
        public Guid? CustomsTransactionGuid { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Side { get; set; }
        public Guid? CustomsTransactionProcGuid { get; set; }
        public string RefRefNumber { get; set; }
        public bool? IsMerged { get; set; }
        public int? VersionNumber { get; set; }
        public Guid? VersionFromGuid { get; set; }
        public Guid? ParentTypeGuid { get; set; }
        public Guid? ParentGuid { get; set; }
        public Guid? PersonGuid { get; set; }
        public Guid? CompanyGuid { get; set; }
        public double? Granting { get; set; }
        public string IDCardNo { get; set; }
        public bool? IsMultiRef { get; set; }
        public Guid? CashAccGuid { get; set; }
        public int? CstKind { get; set; }
        public Guid? NationGuid { get; set; }
        public bool? TaxManually { get; set; }
        public double? TaxManuallyPercent { get; set; }
        public double? AddTax { get; set; }
        public double? FunTax { get; set; }
        public int? Status { get; set; }
        public Guid? DCGuid { get; set; }
        public Guid? MemberGuid { get; set; }
        public double? MemDiscRate { get; set; }
        public bool? IsExternal { get; set; }
        public Guid? PriceOfferGuid { get; set; }
        public Guid? PolicyGuid { get; set; }
        public byte? ShippingKind { get; set; }
        public bool? IsUploaded { get; set; }
        public string EInvoiceGuid { get; set; }
        public DateTime? SupplyDate { get; set; }
       







    }
}
