using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class DocumentPackageInformation
    {
        public string packageId { get; set; }
        public DateTime submissionDate { get; set; }
        public int status { get; set; } //Status of the package preparation. Values - 1: in progress, 2: complete, 3: error, 4: deleted
        public int type { get; set; } //Type of the data to return for each document. Values - 1: full, 2: summary. Full represents the structure of the document including invoice lines. Summary represents only document summary lines.
        public int format { get; set; } //Format of the data to create. Allowed values - 1: CSV (only used for summary requests), 2: XML, 3: JSON
        public int requestorTypeId { get; set; } //Entity which requested the package. Allowed values - 1: Intermediary, 2: Taxpayer,3: Internal User, 4: ERP
        public string requestorTaxpayerRIN { get; set; } //ID of taxpayer used either in issuer ID or receiver ID fields of the document
        public string requestorTaxpayerName { get; set; } //Name of the taxpayer used either in issuer ID or receiver ID fields of the document
        public DateTime deletionDate { get; set; } //Optional: the date and time when it is planned to remove prepared package. Has value only if status is 2: complete
        public bool isExpired { get; set; } //Flag ro indicate if the package is expired
        public DocumentPackageQueryParams queryParameters { get; set; }
    }
}
