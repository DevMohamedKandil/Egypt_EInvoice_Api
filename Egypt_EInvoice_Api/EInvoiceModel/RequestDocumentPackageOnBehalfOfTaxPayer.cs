using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class RequestDocumentPackageOnBehalfOfTaxPayer
    {
        public string type { get; set; } //Type of the data to return for each document. Allowed values - full, summary. Full represents the structure of the document including invoice lines. Summary represents only document summary lines.	
        public string format { get; set; }  //Format of the data to create. Allowed values - CSV (only allowed for summary requests), JSON, XML
        public DocumentPackageQueryParams queryParameters { get; set; }
        public int representedTaxpayerFilterType { get; set; } //This field is a reference to the packages intermediary is requesting, if you set the value to be, ‘1’: this means you are requesting package for all taxpayers you are representing, ‘2’: this means that you are requesting a package on behalf of specific taxpayer & you need to provide his taxpayer RIN, ‘3’: this means you are requesting package for yourself as a taxpayer
        public string representeeRin { get; set; } //(Optional) Registration number for business in Egypt must be registration number
    }
}
