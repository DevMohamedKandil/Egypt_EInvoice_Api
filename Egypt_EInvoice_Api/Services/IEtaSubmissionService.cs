using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.Services
{
    public interface IEtaSubmissionService
    {
        Task<EtaSubmissionResult> SubmitDocumentsAsync(JObject submissionPayload, string billNo, string internalId);
    }
}
