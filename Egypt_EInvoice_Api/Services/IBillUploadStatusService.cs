using Egypt_EInvoice_Api.EInvoiceModel;
using Egypt_EInvoice_Api.Models;
using System;

namespace Egypt_EInvoice_Api.Services
{
    public interface IBillUploadStatusService
    {
        Bill MarkAccepted(Guid billGuid, string submissionUuid, AcceptedDocument acceptedDocument, string rawResponse);
        Bill MarkRejected(Guid billGuid, string status, string reason, string rawResponse);
        Bill MarkDuplicate(Guid billGuid, string message, string rawResponse);
    }
}
