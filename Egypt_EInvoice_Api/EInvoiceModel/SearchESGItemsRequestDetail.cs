using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class SearchESGItemsRequestDetail
    {
        public long codeUsageRequestID { get; set; }
        public string codeTypeName { get; set; }
        public long codeID { get; set; }
        public string itemCode { get; set; }
        public string codeName { get; set; }
        public string description { get; set; }
        public long parentCodeID { get; set; }
        public string parentItemCode { get; set; }
        public string parentLevelName { get; set; }
        public string levelName { get; set; }
        public DateTime requestCreationDateTimeUtc { get; set; }
        public DateTime codeCreationDateTimeUtc { get; set; }
        public DateTime activeFrom { get; set; }
        public DateTime activeTo { get; set; }
        public bool active { get; set; }

        public string status { get; set; }
        public OwnerTaxPayer ownerTaxpayer { get; set; }
        public RequestorTaxpayer requesterTaxpayer { get; set; }
        public CodeCategorization codeCategorization { get; set; }
    }
}
