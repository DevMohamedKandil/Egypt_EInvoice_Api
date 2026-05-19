using Egypt_EInvoice_Api.EInvoiceModel;
using Egypt_EInvoice_Api.Models;
using Egypt_EInvoice_Api.Repos;
using Microsoft.Extensions.Logging;
using System;

namespace Egypt_EInvoice_Api.Services
{
    public class BillUploadStatusService : IBillUploadStatusService
    {
        private const int MaxSubmissionNotesLength = 4000;

        private readonly IBaseRepos<Bill> billRepos;
        private readonly ILogger<BillUploadStatusService> logger;

        public BillUploadStatusService(IBaseRepos<Bill> billRepos, ILogger<BillUploadStatusService> logger)
        {
            this.billRepos = billRepos;
            this.logger = logger;
        }

        public Bill MarkAccepted(Guid billGuid, string submissionUuid, AcceptedDocument acceptedDocument, string rawResponse)
        {
            Bill bill = GetBill(billGuid);
            bill.IsUploaded = true;
            bill.EInvoiceGuid = acceptedDocument?.uuid ?? bill.EInvoiceGuid;
            bill.SubmissionNotes = TrimSubmissionNotes(
                "Accepted by ETA"
                + AppendValue("SubmissionUUID", submissionUuid)
                + AppendValue("DocumentUUID", acceptedDocument?.uuid)
                + AppendValue("DocumentLongId", acceptedDocument?.longId));

            billRepos.Update(bill);

            logger.LogInformation(
                "Bill ETA upload marked accepted. BillGuid: {BillGuid}, BillNo: {BillNo}, SubmissionUUID: {SubmissionUUID}, DocumentUUID: {DocumentUUID}",
                billGuid,
                bill.BillNo,
                submissionUuid,
                acceptedDocument?.uuid);

            return bill;
        }

        public Bill MarkRejected(Guid billGuid, string status, string reason, string rawResponse)
        {
            Bill bill = GetBill(billGuid);
            bill.IsUploaded = false;
            bill.EInvoiceGuid = null;
            bill.SubmissionNotes = TrimSubmissionNotes(
                (string.IsNullOrWhiteSpace(status) ? "Rejected by ETA" : status)
                + AppendValue("Reason", reason));

            billRepos.Update(bill);

            logger.LogWarning(
                "Bill ETA upload marked rejected/failed. BillGuid: {BillGuid}, BillNo: {BillNo}, Status: {EtaStatus}, Reason: {EtaReason}",
                billGuid,
                bill.BillNo,
                status,
                reason);

            return bill;
        }

        public Bill MarkDuplicate(Guid billGuid, string message, string rawResponse)
        {
            Bill bill = GetBill(billGuid);
            if (bill.IsUploaded != true)
                bill.IsUploaded = false;

            bill.SubmissionNotes = TrimSubmissionNotes("Duplicate payload detected by ETA" + AppendValue("Message", message));

            billRepos.Update(bill);

            logger.LogWarning(
                "Bill ETA duplicate payload recorded. BillGuid: {BillGuid}, BillNo: {BillNo}, Message: {EtaMessage}",
                billGuid,
                bill.BillNo,
                message);

            return bill;
        }

        private static string AppendValue(string label, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return "; " + label + ": " + value;
        }

        private static string TrimSubmissionNotes(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            return value.Length <= MaxSubmissionNotesLength
                ? value
                : value.Substring(0, MaxSubmissionNotesLength);
        }

        private Bill GetBill(Guid billGuid)
        {
            Bill bill = billRepos.FindByGuid(billGuid);
            if (bill == null)
                throw new InvalidOperationException("Bill was not found: " + billGuid);

            return bill;
        }
    }
}
