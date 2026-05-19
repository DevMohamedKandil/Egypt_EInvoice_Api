using Egypt_EInvoice_Api.EInvoiceModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.Services
{
    public class EtaAuthService : IEtaAuthService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<EtaAuthService> logger;

        public EtaAuthService(IConfiguration configuration, ILogger<EtaAuthService> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            Appsettings settings = configuration.GetRequiredSection("Settings").Get<Appsettings>();
            string tokenEndpoint = settings.loginUrl.TrimEnd('/') + "/connect/token";

            logger.LogInformation("ETA token retrieval started. TokenEndpoint: {TokenEndpoint}", tokenEndpoint);

            using (HttpClient client = new HttpClient())
            using (HttpContent content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", settings.ClientId),
                new KeyValuePair<string, string>("client_secret", settings.ClientSecret),
                new KeyValuePair<string, string>("scope", "InvoicingAPI")
            }))
            {
                if (!string.IsNullOrWhiteSpace(settings.onBehalf))
                    client.DefaultRequestHeaders.Add("onbehalfof", settings.onBehalf);

                HttpResponseMessage response = await client.PostAsync(tokenEndpoint, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError(
                        "ETA token retrieval failed. StatusCode: {StatusCode}, Response: {EtaResponse}",
                        response.StatusCode,
                        responseBody);

                    throw new InvalidOperationException("ETA token request failed: " + responseBody);
                }

                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseBody);
                if (loginResponse == null || string.IsNullOrWhiteSpace(loginResponse.access_token))
                    throw new InvalidOperationException("ETA token response did not include an access token.");

                logger.LogInformation("ETA token retrieval completed. ExpiresInSeconds: {ExpiresInSeconds}", loginResponse.expires_in);
                return loginResponse.access_token;
            }
        }
    }
}
