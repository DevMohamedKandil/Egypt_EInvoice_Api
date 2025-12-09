using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class PublishedCodeResponse
    {
        public long codeID { get; set; }
        public string CodeLookupValue { get; set; }
        public string codeNamePrimaryLang { get; set; }
        public string codeNameSecondaryLang { get; set; }
        public string codeDescriptionPrimaryLang { get; set; }
        public string codeDescriptionSecondaryLang { get; set; }
        public DateTime activeFrom { get; set; }
        public DateTime activeTo { get; set; }
        public long parentCodeID { get; set; }
        public string ParentCodeLookupValue { get; set; }
        public long codeTypeID { get; set; }
        public long CodeTypeLevelID { get; set; }
        public string codeTypeLevelNamePrimaryLang { get; set; }
        public string codeTypeLevelNameSecondaryLang { get; set; }
        public string parentCodeNamePrimaryLang { get; set; }
        public string parentCodeNameSecondaryLang { get; set; }
        public string parentLevelName { get; set; }
        public string codeTypeNamePrimaryLang { get; set; }
        public bool active { get; set; }
        public string linkedCode { get; set; }
    }
}
