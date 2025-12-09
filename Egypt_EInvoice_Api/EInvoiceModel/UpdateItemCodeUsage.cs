using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class UpdateItemCodeUsage
    {
        public string itemCode { get; set; }
        public string codeName { get; set; }
        public string codeNameAr { get; set; }
        public DateTime activeFrom { get; set; }
        public DateTime activeTo { get; set; }
        public string description { get; set; }
        public string descriptionAr { get; set; }
        public string parentCode { get; set; }
        public string requestReason { get; set; }
        public string linkedCode { get; set; }
    }
}
