using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Egypt_EInvoice_Api.Models
{
    public class BillType
    {

        [Key]
        public Guid Guid { get; set; }
        public int Number { get; set; }
        public string Kind { get; set; }
        public int KindID { get; set; }
        public string Name { get; set; }
        public string LatinName { get; set; }

        public bool? NotPosted { get; set; }

        public bool? AutoPosted { get; set; }

        public bool? TransBill { get; set; }


        public bool? WorkBill { get; set; }
        public bool? NotEntry { get; set; }

        public bool? AutoEntry { get; set; }


        public bool? AutoAccPosted { get; set; }
        public int? AccMatNumber { get; set; }
        public int? StoreNumber { get; set; }
        public Guid? AccMatGuid { get; set; }
        public Guid? StoreGuid { get; set; }
        public Guid? RElatedGuid { get; set; }
        public int? DefPrice { get; set; }
        public bool? AffectCostPrice { get; set; }
        public Guid? CustAccGuid { get; set; }
        public int? PayType { get; set; }

        public bool? InvBill { get; set; }

        public bool? StkTrans { get; set; }

        public Guid? GroupGUID { get; set; }
        public int? GroupID { get; set; }
        public Guid? DelegateGUID { get; set; }
        public bool? PoBill { get; set; }
        public int? Flag { get; set; }
        public bool? ExtraAffectCost { get; set; }
        public bool? DiscAffectCost { get; set; }
        public int? DefUnit { get; set; }
        public int? Branch { get; set; }
        public bool? PrOnLine { get; set; }
        public bool? OpenDrawer { get; set; }
        public Guid? CostGUID { get; set; }
        public int? CostType { get; set; }
        public int? Period { get; set; }

        public Guid? CostAccGuid { get; set; }

        public Guid? StkAccGuid { get; set; }
        public int? CurPtr { get; set; }
        public Guid? PayTypeGuid { get; set; }
        public Guid? DefRefTypeGuid { get; set; }
        public bool? RefreshPrice { get; set; }
        public bool? MergeExistingMats { get; set; }
        public bool? GetDataOnSelectRefNo { get; set; }
        public bool? SnSeparateLine { get; set; }
        public string SnSeparator { get; set; }
        public bool? GetRefRemark { get; set; }
        public bool? GetRefPayConditions { get; set; }
        public bool? GetRefAddsDiscounts { get; set; }
        public bool? ApplyRefAddsDiscounts { get; set; }
        public bool? ConnectPayConditionsToNetBill { get; set; }
        public bool? ApplyRefMatsAddsDiscounts { get; set; }
        public bool? AutoOutOldExpDate { get; set; }
        public bool? RoundValues { get; set; }
        public int? ValuesDecimalCount { get; set; }
        public bool? RoundQtyValues { get; set; }
        public int? QtyDecimalCount { get; set; }
        public int? SnCount { get; set; }
        public bool? BringRefStore { get; set; }
        public bool? OutExpDateByPurchasesPrice { get; set; }
        public bool? BringPurchaseDiscForExpDate { get; set; }
        public bool? GenerateTranBill { get; set; }
        public bool? ShowTranBill { get; set; }
        public Guid? TranStoreGuid { get; set; }
        public Guid? InBillTypeGuid { get; set; }
        public Guid? OutBillTypeGuid { get; set; }
        public bool? ModifyQtyWhenModifyRealQty { get; set; }
        public bool? ModifyPriceWhenModifyTotal { get; set; }
        public bool? CustomsClearanceBill { get; set; }
        public bool? BringRefEntryValues { get; set; }
        public int? ColorNormal { get; set; }
        public int? ColorLessMinCost { get; set; }

        public int? ColorLessMinSales { get; set; }

        public int? TransDefPrice { get; set; }
        public int? QtyGreaterMaxColor { get; set; }
        public int? QtyLessMinColor { get; set; }
        public int? QtyLessZeroColor { get; set; }
        public int? FontMultiUnit { get; set; }
        public int? InStkBranch { get; set; }
        public int? IsColored { get; set; }
        public bool? RentingBill { get; set; }
        public bool? BalanceAcc { get; set; }
        public bool? RelateWithCustomsTransaction { get; set; }
        public bool? UpdateDataAfterSave { get; set; }
        public Guid? GroupedBillType { get; set; }
        public bool? ShowRefRefNo { get; set; }
        public bool? GetRefAddRecords { get; set; }
        public bool? MultiVersion { get; set; }
        public int? VerColCount { get; set; }
        public bool? GetRefCost { get; set; }
        public bool? FilterByGroup { get; set; }
        public bool? MaxRow { get; set; }
        public int? MaxRowCount { get; set; }
        public bool? PrintMatPicture { get; set; }
        public bool? FilterPersonByAcc { get; set; }
        public bool? FilterCompByAcc { get; set; }
        public bool? CodeByUser { get; set; }
        public Guid? GrantingAccGuid { get; set; }
        public bool? CopyValToPaid { get; set; }
        public bool? AllowMultiRef { get; set; }
        public bool? KeepRefNo { get; set; }
        public bool? GetTotalRefQtyInfo { get; set; }
        public bool? SimpleWork { get; set; }
        public int? ColorNegMat { get; set; }
        public Guid? PosDiscAccGuid { get; set; }
        public Guid? PosExtraAccGuid { get; set; }
        public int? CstKind { get; set; }
        public Guid? NationGuid { get; set; }
        public Guid? PaidAccGuid { get; set; }
        public Guid? RemainAccGuid { get; set; }
        public bool? UpdateExpDate { get; set; }
        public Guid? AddTaxAccGuid { get; set; }
        public Guid? FunTaxAccGuid { get; set; }
        public bool? CalcTaxAfterDiscAndExtra { get; set; }
        public bool? UnCalcTax { get; set; }
        public bool? ComBill { get; set; }
        public bool? ConfirmEmp { get; set; }
        public int? CalcComCollect { get; set; }
        public int? CollectMaxPeriod { get; set; }
        public string ComStructureEmp { get; set; }
        public bool? RelateEmpsCarsTrailer { get; set; }
        public bool? WorkTouch { get; set; }
        public bool? ManuallyTax { get; set; }
        public double? TaxPercent { get; set; }
        public bool? GenBillsByMatStore { get; set; }
        public bool? RelateMatWithDefStore { get; set; }
        public bool? BringRefMainStore { get; set; }
        public bool? BringRefPayType { get; set; }
        public bool? BringBranchRef { get; set; }
        public bool? GetRefDetRemark { get; set; }
        public bool? GetRefOppAcc { get; set; }
        public bool? GetRefBillRef { get; set; }
        public bool? BringRefEntryValuesWithoutTax { get; set; }
        public bool? BringBillRelated { get; set; }
        public bool? PrintByMatClass { get; set; }
        public Int64? BillMatClassDesign { get; set; }
        public Int64? POSBillMatClassDesign { get; set; }
        public bool? CalcAddTaxAfterFunTax { get; set; }
        public bool? ModifyCarWeight { get; set; }
        public bool? BringCurrentContentSalePrice { get; set; }
        public bool? ColorFromMatCard { get; set; }
        public int? MatColor { get; set; }
        public int? ClassColor { get; set; }
        public string MFName { get; set; }
        public int? MFColor { get; set; }
        public int? MFSize { get; set; }
        public int? MFHeight { get; set; }
        public int? MFStyle { get; set; }
        public string CFName { get; set; }
        public int? CFColor { get; set; }
        public int? CFSize { get; set; }
        public int? CFHeight { get; set; }
        public int? CFStyle { get; set; }
        public bool? DoNotShowMatCaptionWithImage { get; set; }
        public bool? WaitingRec { get; set; }
        public byte? WaitingRecKind { get; set; }
        public int? WaitingRecFrom { get; set; }
        public int? WaitingRecTo { get; set; }
        public int? HorzMatCount { get; set; }
        public int? VertMatCount { get; set; }
        public bool? ShowMemberShortcut { get; set; }
        public bool? ShowUpClassShortcut { get; set; }
        public bool? ComposedBarcode { get; set; }
        public byte? ComposedBarcodeKind { get; set; }
        public bool? SearchInRefMat { get; set; }
        public bool? SearchInUnFinishRefMat { get; set; }
        public Guid? PriceOfferGuid { get; set; }
        public bool? CalcPaidInBillPay { get; set; }
        public int? ColorOfferPrice { get; set; }
        public bool? BringRefAddData { get; set; }
        public Guid? RefQualiyFieldGuid { get; set; }
        public string QualityItemGuids { get; set; }
        public bool? RefreshRowTax { get; set; }
        public bool? ShowRefCode { get; set; }
        public bool? GenStoreBill { get; set; }
        public bool? Project { get; set; }
        public bool? ShowPersonShortcut { get; set; }
        public bool? GetRefPeriod { get; set; }
        public bool? ShowMatTableShortcut { get; set; }
        public bool? ShowDiscShortCut { get; set; }
        public bool? DoNotBringServiceMat { get; set; }
        public Guid? QuickExchangeTypeGuid { get; set; }
        public int? DiscExtraDistWay { get; set; }
        public bool? BringDefineDefaultValue { get; set; }
        public bool? ApplySourceTax { get; set; }
        public double? SourceTaxPercent { get; set; }
        public byte? SourceTaxType { get; set; }
        public byte? ManuallyTaxType { get; set; }
        //public bool? ElectBill { get; set; }
        //public Int64? ElectBillDesign { get; set; }
      







    }
}
