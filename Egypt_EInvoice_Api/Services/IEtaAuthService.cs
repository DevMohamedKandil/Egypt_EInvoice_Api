using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.Services
{
    public interface IEtaAuthService
    {
        Task<string> GetAccessTokenAsync();
    }
}
