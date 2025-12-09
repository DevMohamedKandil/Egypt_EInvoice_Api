using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class DocumentSummary
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
        public DateTime? cancelRequestDate { get; set; }
        public DateTime? rejectRequestDate { get; set; }
        public DateTime? cancelRequestDelayedDate { get; set; } //Refer to the date when this document will be marked as cancelled if the receiver taxpayer didn’t decline the cancellation reuest that was raised by issuer taxpayer, will be in UTC format
        public DateTime? rejectRequestDelayedDate { get; set; } //	Refer to the date when this document will be marked as rejected if the issuer taxpayer didn’t decline the rejection request that was raised by receiver taxpayer, will be in UTC format
        public DateTime? declineCancelRequestDate { get; set; } //Refer to the decline cancellation request that has been initiated by the receiver taxpayer on the system, will be in UTC format
        public DateTime? declineRejectRequestDate { get; set; } //Refer to the decline rejection request that has been initiated by the issuer taxpayer of the document on the system, will be in UTC format

    }
}
