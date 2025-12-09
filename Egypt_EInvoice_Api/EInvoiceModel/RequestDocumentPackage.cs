using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class RequestDocumentPackage
    {
        public string type { get; set; } //Type of the data to return for each document. Allowed values - full, summary. Full represents the structure of the document including invoice lines. Summary represents only document summary lines.	
        public string format { get; set; }  //Format of the data to create. Allowed values - CSV (only allowed for summary requests), JSON, XML
        public DocumentPackageQueryParams queryParameters { get; set; }
    }
}
