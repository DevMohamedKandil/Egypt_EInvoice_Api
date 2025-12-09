using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Egypt_EInvoice_Api.Models
{
    public class Group
    {
        [Key]
        public Guid Guid { get; set; }
        public int Number { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string LatinName { get; set; }
        public Guid? ParentGuid { get; set; }
        public int? ParentNumber { get; set; }
        public string DefUnit { get; set; }
        public string Notes { get; set; }
        public int? Branch { get; set; }
        public Guid? AccGuid { get; set; }
        public Guid? CostAccGuid { get; set; }
        public Guid? StkAccGuid { get; set; }
        public int? Kind { get; set; }
        public int? CalcWay { get; set; }
        public Guid? AssetsGuid { get; set; }
        public Guid? StGuid { get; set; }
        public double? CashComm { get; set; }
        public double? LaterComm { get; set; }
        public int? CommType { get; set; }
        public int? BuyDiscType { get; set; }
        public double? BuyDiscRate { get; set; }
        public double? BuyDiscVal { get; set; }
        public int? DiscAffectCost { get; set; }
        public int? SaleDiscType { get; set; }
        public double? SaleDiscRate { get; set; }
        public double? SaleDiscVal { get; set; }
        public double? BuyFQRate { get; set; }
        public double? SaleFQRate { get; set; }
        public string ImgPath { get; set; }
        public string ImgExt { get; set; }
        public Guid? PurAccGuid { get; set; }
        public Guid? ReSaleAccGuid { get; set; }
        public Guid? RePurAccGuid { get; set; }
        public double? AddTaxRate { get; set; }
        public Guid? AddTaxSaleAccGuid { get; set; }
        public Guid? AddTaxPurAccGuid { get; set; }
        public double? FunTaxRate { get; set; }
        public Guid? FunTaxSaleAccGuid { get; set; }
        public Guid? FunTaxPurAccGuid { get; set; }
        public bool? Taxable { get; set; }
        public Guid? SecondStoreGuid { get; set; }
        public int? TaxableType { get; set; }
    }
}
