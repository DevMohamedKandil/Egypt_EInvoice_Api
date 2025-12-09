using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class PackageRequestLog
    {
        public List<DocumentPackageInformation> result { get; set; }
        public List<DocumentMetadata> metadata { get; set; }
    }
}
