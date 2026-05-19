using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.Services
{
    public class EtaSubmissionService : IEtaSubmissionService
    {
        private readonly IConfiguration configuration;
        private readonly IEtaAuthService authService;
        private readonly ILogger<EtaSubmissionService> logger;

        public EtaSubmissionService(
            IConfiguration configuration,
            IEtaAuthService authService,
            ILogger<EtaSubmissionService> logger)
        {
            this.configuration = configuration;
            this.authService = authService;
            this.logger = logger;
        }

        public async Task<EtaSubmissionResult> SubmitDocumentsAsync(JObject submissionPayload, string billNo, string internalId)
        {
            Appsettings settings = configuration.GetRequiredSection("Settings").Get<Appsettings>();
            string submissionEndpoint = settings.apiUrl.TrimEnd('/') + "/api/v1.0/documentsubmissions";

            string accessToken = await authService.GetAccessTokenAsync();
            string payloadJson = submissionPayload.ToString(Formatting.None);

            logger.LogInformation(
                "ETA submission started. BillNo: {BillNo}, InternalId: {InternalId}, Endpoint: {SubmissionEndpoint}, PayloadJson: {PayloadJson}",
                billNo,
                internalId,
                submissionEndpoint,
                payloadJson);

            using (HttpClient client = new HttpClient())
            using (HttpContent content = new StringContent(payloadJson, Encoding.UTF8, "application/json"))
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                if (!string.IsNullOrWhiteSpace(settings.onBehalf))
                    client.DefaultRequestHeaders.Add("onbehalfof", settings.onBehalf);

                HttpResponseMessage response = await client.PostAsync(submissionEndpoint, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                EtaSubmissionResult result = new EtaSubmissionResult
                {
                    StatusCode = response.StatusCode,
                    RawResponse = responseBody,
                    IsDuplicatePayload = IsDuplicatePayload(responseBody)
                };

                try
                {
                    if (!string.IsNullOrWhiteSpace(responseBody))
                        result.Response = JsonConvert.DeserializeObject<EInvoiceModel.DocumetSubmitResponse>(responseBody);
                }
                catch (JsonException ex)
                {
                    result.ErrorMessage = responseBody;
                    logger.LogWarning(
                        ex,
                        "ETA response was not a document submission response. BillNo: {BillNo}, InternalId: {InternalId}, StatusCode: {StatusCode}",
                        billNo,
                        internalId,
                        response.StatusCode);
                }

                if (result.IsDuplicatePayload)
                {
                    result.ErrorMessage = "ETA duplicate payload: request payload is identical to a previous payload sent in the last 10 minutes.";
                    logger.LogWarning(
                        "ETA duplicate payload detected. BillNo: {BillNo}, InternalId: {InternalId}, Response: {EtaResponse}",
                        billNo,
                        internalId,
                        responseBody);
                }

                logger.LogInformation(
                    "ETA submission completed. BillNo: {BillNo}, InternalId: {InternalId}, StatusCode: {StatusCode}, SubmissionUUID: {SubmissionUUID}, AcceptedCount: {AcceptedCount}, RejectedCount: {RejectedCount}, RawResponse: {RawResponse}",
                    billNo,
                    internalId,
                    response.StatusCode,
                    result.Response?.submissionUUID,
                    result.Response?.acceptedDocuments?.Count ?? 0,
                    result.Response?.rejectedDocuments?.Count ?? 0,
                    responseBody);

                return result;
            }
        }

        private static bool IsDuplicatePayload(string responseBody)
        {
            return !string.IsNullOrWhiteSpace(responseBody)
                && responseBody.IndexOf("identical to a previous payload", StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
