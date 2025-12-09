using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Egypt_EInvoice_Api.Models
{
    public class User
    {
        public Guid? GUID { get; set; }
        [Key]
        public int Number { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string LatinName { get; set; }
        public string EntryName { get; set; }
        public string Pass { get; set; }
        public bool IsAdmin { get; set; }

        public int? SaveDays { get; set; }
        public DateTime? Savedate { get; set; }
        public bool CostPrice { get; set; }
        public bool AccDebt { get; set; }
        public bool MatBal { get; set; }
        public int? Branch { get; set; }
        public int? Flag { get; set; }
        public bool? NewFile { get; set; }
        public int? Reports { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public bool? AccBal { get; set; }
        public bool? BranchAccBal { get; set; }
        public bool? PostedAccBal { get; set; }
        public int? DirectManager { get; set; }
        public int? UserBranch { get; set; }
        public Guid? ManagGuid { get; set; }
        public Guid? DepGuid { get; set; }
        public Guid? JobTypeGuid { get; set; }
    }
}
