using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class ItemCodeDetail
    {
        public long codeID { get; set; }
        public string codeName { get; set; }
        public string codeNameAr { get; set; }
        public string description { get; set; }
        public string descriptionAr { get; set; }
        public DateTime activeFrom { get; set; }
        public DateTime activeTo { get; set; }
        public string ParentCodeLookupValue { get; set; }
        public long codeTypeID { get; set; }
        public long CodeTypeLevelID { get; set; }
        public string codeTypeLevelNamePrimaryLang { get; set; }
        public string codeTypeLevelNameSecondaryLang { get; set; }

        public string parentItemCode { get; set; }
        public long ParentCodeID { get; set; }
        public string parentCodeName { get; set; }
        public string parentCodeNameAr { get; set; }
        public string parentLevelName { get; set; }
        public DateTime parentActiveFrom { get; set; }
        public DateTime parentActiveTo { get; set; }
        public string parentDescription { get; set; }
        public string parentDescriptionAr { get; set; }
        public bool parentActive { get; set; }
        public bool active { get; set; }
    }
}
