using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class DocumentExtended
    {
        public string uuid { get; set; }
        public string submissionUUID { get; set; }
        public string longId { get; set; }
        public string internalId { get; set; }
        public string typeName { get; set; } //i (for invoice), c (for credit note), d (for debit note)
        public string typeVersionName { get; set; }
        public string issuerId { get; set; } //Registration number of issuer
        public string issuerName { get; set; }
        public string receiverId { get; set; } //Optional: receiver registration number (can be national ID or foreigner ID).
        public string receiverName { get; set; }
        public DateTime dateTimeIssued { get; set; }
        public DateTime dateTimeReceived { get; set; }
        public decimal totalSales { get; set; }
        public decimal totalDiscount { get; set; }
        public decimal netAmount { get; set; }
        public decimal total { get; set; }
        public string status { get; set; } //Status of the document - “submitted”, “valid”, “invalid”, “rejected”, “cancelled”
        public Document document { get; set; }
        public string transformationStatus { get; set; } //Flag defining if the document has been transformed from XML to JSON or the other way depending on what was requested and what was the original format. Values: original, transformed. Note that transformed documents will have signature invalid due to format change.
        public DocumentValidationResult validationResults { get; set; }
    }
}
