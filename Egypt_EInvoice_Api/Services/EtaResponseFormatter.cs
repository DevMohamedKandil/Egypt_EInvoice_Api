using Egypt_EInvoice_Api.EInvoiceModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Egypt_EInvoice_Api.Services
{
    public static class EtaResponseFormatter
    {
        public static string BuildRejectedDocumentsMessage(IEnumerable<RejectedDocument> rejectedDocuments)
        {
            List<string> messages = new List<string>();

            foreach (RejectedDocument rejectedDocument in rejectedDocuments ?? Enumerable.Empty<RejectedDocument>())
            {
                List<string> documentMessages = new List<string>();
                CollectMessages(rejectedDocument.error, documentMessages);

                if (documentMessages.Count == 0)
                    documentMessages.Add("Document was rejected by ETA.");

                foreach (string message in documentMessages)
                {
                    messages.Add(string.IsNullOrWhiteSpace(rejectedDocument.internalId)
                        ? message
                        : rejectedDocument.internalId + ": " + message);
                }
            }

            return string.Join(Environment.NewLine, messages.Distinct());
        }

        public static string BuildRawFailureMessage(EtaSubmissionResult result)
        {
            if (!string.IsNullOrWhiteSpace(result?.ErrorMessage))
                return result.ErrorMessage;

            if (!string.IsNullOrWhiteSpace(result?.RawResponse))
                return result.RawResponse;

            return "Upload process failed, Please try again later";
        }

        private static void CollectMessages(Error error, IList<string> messages)
        {
            if (error == null)
                return;

            if (!string.IsNullOrWhiteSpace(error.message))
                messages.Add(error.message);

            if (error.details == null)
                return;

            foreach (Error detail in error.details)
            {
                CollectMessages(detail, messages);
            }
        }
    }
}
