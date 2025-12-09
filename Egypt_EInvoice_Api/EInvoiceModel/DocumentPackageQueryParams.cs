using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class DocumentPackageQueryParams
    {
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public string[] documentTypeNames { get; set; } //Optional: Array of document type requested. Allowed values - i, c, d
        public string[] statuses { get; set; } //Optional: Array of document statuses to filter. Allowed values - valid, invalid, rejected, cancelled
        public string[] productsInternalCodes { get; set; } //Optional: Array of internal code used for the product being sold, exact match
        public string receiverSenderType { get; set; } //Type of the other entity 0(Business), 1(Person), 2(Foreign)
        public string receiverSenderId { get; set; } //Optional: Value depends on the selected ‘receiverSenderType’. RIN of Taxpayer (Business), National ID (Person), ID (Foreign) used either in issuer ID or receiver ID fields of the document

        public string branchNumber { get; set; } //Optional: Branch Number of the issuer
        public List<ItemCode> itemCodes { get; set; } //Optional: Array of the item codes used in filtering line items of the document
    }
}
