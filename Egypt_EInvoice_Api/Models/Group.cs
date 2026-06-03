using System;
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

        public string DefUnit { get; set; }
        public string Notes { get; set; }

        public int? Branch { get; set; }

        public double? CashComm { get; set; }
        public double? LaterComm { get; set; }

        public double? BuyDiscRate { get; set; }
        public double? BuyDiscVal { get; set; }

        public double? SaleDiscRate { get; set; }
        public double? SaleDiscVal { get; set; }

        public double? AddTaxRate { get; set; }
        public double? FunTaxRate { get; set; }

        public bool? Taxable { get; set; }
    }
}