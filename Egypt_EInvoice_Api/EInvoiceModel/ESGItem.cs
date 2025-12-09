using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class ESGItem
    {
        public string codeType { get; set; } //Refer to the catalog where this code will be generated, EGS catalog is mainly used by taxpayers to register their internal codes
        public string parentCode { get; set; } //The parent of the code, represents level 4 GPC code
        public string itemCode { get; set; } //Refer to the internal code of the taxpayer that need to be registerd on eInvoicing solution, format for the code should follow the following standard “EG-TaxpayerID-InternalCode”
        public string codeName { get; set; }
        public string codeNameAr { get; set; }
        public DateTime activeFrom { get; set; }
        public DateTime activeTo { get; set; }
        public string description { get; set; }
        public string descriptionAr { get; set; }
        public string requestReason { get; set; }
        public string linkedCode { get; set; }
    }
}
