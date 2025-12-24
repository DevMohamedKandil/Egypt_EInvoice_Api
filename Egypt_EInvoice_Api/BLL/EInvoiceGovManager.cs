using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Egypt_EInvoice_Api.EInvoiceModel;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Egypt_EInvoice_Api.Response;

namespace Egypt_EInvoice_Api.BLL
{
    public class EInvoiceGovManager
    {
        //private readonly string loginUrl;
        //private readonly string apiUrl;
        //private readonly string client_id;
        //private readonly string client_secret;
        //private readonly string onBehalfOf; // الدخول بالنيابة عن وتأخذ رقم السجل التجاري
        private  string token;

        private HttpClient httpClient;
        private HttpClient requestHttpClient;

    
        public EInvoiceGovManager()
        {
            IConfiguration config = new ConfigurationBuilder()
.AddJsonFile("appsettings.json")
.AddEnvironmentVariables()
.Build();


            Appsettings settings = config.GetRequiredSection("Settings").Get<Appsettings>();

            httpClient = new HttpClient();
            requestHttpClient = new HttpClient();
            requestHttpClient.BaseAddress = new Uri(settings.apiUrl);
          
           // this.onBehalfOf = onBehalfOf;
            //this.requestHttpClient = requestHttpClient;

            //this.httpClient = httpClient;
            byte[] encodingAuthorization = Encoding.ASCII.GetBytes(settings.ClientId + ":" + settings.ClientSecret);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(encodingAuthorization));
            httpClient.BaseAddress = new Uri(settings.loginUrl);

        }

        public LoginResponse Login()
        {
            IConfiguration config = new ConfigurationBuilder()
.AddJsonFile("appsettings.json")
.AddEnvironmentVariables()
.Build();


            Appsettings settings = config.GetRequiredSection("Settings").Get<Appsettings>();

            HttpContent content = new FormUrlEncodedContent(
                //   null
                new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", settings.ClientId),
                    new KeyValuePair<string, string>("client_secret", settings.ClientSecret),
                    new KeyValuePair<string, string>("scope", "InvoicingAPI"),
                }
                );

            var response = this.httpClient.PostAsync("/connect/token", content);
            var result = response.GetAwaiter().GetResult();
            if (result.IsSuccessStatusCode)
            {
                string responseData = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseData);
                token = loginResponse.access_token;
                requestHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token );
                return loginResponse;

            }
            return null;


        }

        public LoginResponse LoginIntermediary()
        {
            IConfiguration config = new ConfigurationBuilder()
.AddJsonFile("appsettings.json")
.AddEnvironmentVariables()
.Build();


            Appsettings settings = config.GetRequiredSection("Settings").Get<Appsettings>();
            httpClient.DefaultRequestHeaders.Add("onbehalfof", settings.onBehalf);
            HttpContent content = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });
            var result = httpClient.PostAsync("/connect/token", content).GetAwaiter().GetResult();
            if (result.IsSuccessStatusCode)
            {
                string responseData = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseData);
                return loginResponse;
            }
            return null;

        }

        

        public string GetAllDocumentTypes()
        {
            
            
            var result = requestHttpClient.GetAsync("/api/v1.0/documenttypes").GetAwaiter().GetResult();
            if (result.IsSuccessStatusCode)
            {
                var response = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return response;
            }

            return "";
        }

        public void GetDocumnetTypeById(int id)
        {
            var result = this.requestHttpClient.GetAsync("/api/v1.0/documenttypes/" + id);
        }

        public void GetDocumnetTypeVersion(int documentId, int versionId)
        {
            requestHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var result = this.requestHttpClient.GetAsync("/api/v1.0/documenttypes/" + documentId + "/versions/" + versionId);

        }

        public void SubmitDocument(List<_documents> documents)
        {
            try
            {
                for (int i = 0; i < documents.Count; i++)
                {
                    string output = JsonConvert.SerializeObject(documents);
                    

                    
                    //StringBuilder str = new StringBuilder();
                   

                    //  output.;

                    //  output = output.Remove.;
                    // output = output.Replace("]", "}").TrimEnd();


                    //Documents _Document = new Documents()
                    //{
                    //    Document = documents.ToArray()
                    //};

                    //v1.0
                    var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1.0/documentsubmissions/")
                    {
                        Content = JsonContent.Create(documents)
                    };

                 

                    var result = requestHttpClient.SendAsync(request);
                    var response = result.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (result.IsCompletedSuccessfully == true)
                    {

                    }
                }
            }
            catch (Exception e)
            {

                throw e.InnerException;
            }
            //JsonContent.Create()

            // requestHttpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            //var request = new HttpRequestMessage(HttpMethod.Post,Helper.apiUrl + "/api/v1.0/documentsubmissions/")
            //{
            //    Content = JsonContent.Create(documents)
            //};
            //var result = this.requestHttpClient.SendAsync(request).GetAwaiter().GetResult();
            //if (result.IsSuccessStatusCode)
            //{
            //    return JsonConvert.DeserializeObject<DocumetSubmitResponse>(result.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            //}
            //return null;

            //var result = this.requestHttpClient.PostAsJsonAsync("/api/v1.0/documentsubmissions/")

           
        }

        public void CancelOrRejectDocument(string uuid, string reason, string status)
        {
            /* Path  === /api/v1.0/documents/{UUID}/state 
             */

            //status = cancelled or rejected
            DocumentCancelOrReject documentCancel = new DocumentCancelOrReject()
            {
                status = status,
                reason = reason
            };
            var request = new HttpRequestMessage(HttpMethod.Put, "Full Url Here")
            {
                Content = JsonContent.Create(documentCancel)
            };

            var resutl = this.requestHttpClient.SendAsync(request);


        }


        public void GetRecentDocument(int pageNo, int pageSize)
        {
            /* path===
             /api/v1.0/documents/recent?pageNo={pageNo}&pageSize={pageSize}              
             */
            var result = this.requestHttpClient.GetAsync("/api/v1.0/documents/recent?pageNo=" + pageNo + "&pageSize=" + pageSize);

        }

        public void GetRequestDocumentPackage(RequestDocumentPackage requestDocumentPackage) //API allows taxpayer system to request larger packages of previously sent or received. it will download file (CSV or XML or JSON)
        {
            /*Path == 
             Signature: POST /api/v1.0/documentpackages/requests             
             */
            var request = new HttpRequestMessage(HttpMethod.Post, "Full Url Here")
            {
                Content = JsonContent.Create(requestDocumentPackage)
            };
            var result = this.requestHttpClient.SendAsync(request);

        }

        public void RequestDocumentPackageOnBehalfOfTaxPayer(RequestDocumentPackageOnBehalfOfTaxPayer requestDocumentPackageOn)
        {
            /*Path == 
                 Signature: POST /api/v1.0/documentpackages/requests             
                 */
            var request = new HttpRequestMessage(HttpMethod.Post, "Full Url Here")
            {
                Content = JsonContent.Create(requestDocumentPackageOn)
            };
            var result = this.requestHttpClient.SendAsync(request);




        }

        public void GetPackageRequestLog(int pageNo, int pageSize)
        {

            /*
             * Signature: GET /api/v1.0/documentpackages/requests?pageNo={pageNo}&pageSize={pageSize}
             */
            var result = this.requestHttpClient.GetAsync("/api/v1.0/documentpackages/requests?pageNo=" + pageNo + "&pageSize=" + pageSize);
        }


        public void GetDocumentPackage(string rid) //API allows taxpayer systems to download previously requested document packages as XML or JSON files (compressed).
        {
            //rid is request Id
            /*
             * Signature: GET /api/v1.0/documentpackages/{rid}
             */
            this.requestHttpClient.DefaultRequestHeaders.Add("Content-Type", "application/octet-stream");
            this.requestHttpClient.DefaultRequestHeaders.Add("Content-Length", "8838849");
            var result = this.requestHttpClient.GetAsync("/api/v1.0/documentpackages/" + rid);


        }

        public void GetDocumentByUUID(string uuid)
        {
            /*
             Signature: GET /api/v1.0/documents/{uuid}/raw
             */

            var result = this.requestHttpClient.GetAsync("/api/v1.0/documents/" + uuid + "/raw");

        }

        public void GetSubmission(string uuid, int pageNo, int pageSize) //API returns information on documents submitted during a single submission by taxpayer.
        {

            /*
             * Signature: GET /api/v1.0/documentsubmissions/{uuid}?pageNo={pageNo}&pageSize={pageSize}
             */
            var result = this.requestHttpClient.GetAsync("/api/v1.0/documentsubmissions/" + uuid + "?pageNo=" + pageNo + "&pageSize=" + pageSize);

        }

        public void GetDocumentPrintout(string uuid) //API enables ERP system to download the PDF version of the document having predefined layout.
        {
            /*
             Signature: GET /api/v1.0/documents/{uuid}/pdf
             */
            this.requestHttpClient.DefaultRequestHeaders.Add("Content-Type", "application/octet-stream");
            this.requestHttpClient.DefaultRequestHeaders.Add("Content-Length", "1123249");
            var result = requestHttpClient.GetAsync("/api/v1.0/documents/" + uuid + "/pdf");

        }

        public void EnableReceiveDocumentNotifications(DocumentNotificationMessage documentNotificationMessage)
        {
            /*
             * Types of notifications delivered are:
               document validated (validation complete and document is marked valid or invalid)
               document received
               document rejected
               document cancelled
             */

            /*
             *Signature:PUT /notifications/documents
             */

            var request = new HttpRequestMessage(HttpMethod.Put, "Full URL")
            {
                Content = JsonContent.Create(documentNotificationMessage),


            };
            request.Headers.Add("Content-Type", "application/json");
            request.Headers.Add("Accept-Language", "ar");
            this.requestHttpClient.SendAsync(request);

        }

        public void EnableReceiveDownloadReadyNotification(DocumentPackageReadyNotification documentPackageReadyNotification) //API needs to be exposed by ERP and allows ERP system to receive notification when document package is ready for download
        {

            /*
             * Singature:PUT /notifications/documentpackages
             */

            var request = new HttpRequestMessage(HttpMethod.Put, "Full URL")
            {
                Content = JsonContent.Create(documentPackageReadyNotification),


            };
            request.Headers.Add("Content-Type", "application/json");
            request.Headers.Add("Accept-Language", "ar");
            this.requestHttpClient.SendAsync(request);
        }

        public void GetNotification(DateTime dateFrom, DateTime dateTo, string type, string language, string status, string chanel, int pageNo, int pageSize) //API allows ERP system to query for previously received notifications
        {
            /*
                Signature: GET /api/v1.0/notifications/taxpayer?dateFrom={dateFrom}&dateTo={dateTo}&type={type}&language={language}&status={status}&channel={channel}&pageNo={pageNo}&pageSize={pageSize}
             */

            var result = requestHttpClient.GetAsync("/api/v1.0/notifications/taxpayer?dateFrom=" + dateFrom + "&dateTo=" + dateTo + "&type=" + type + "&language=" + language + "&status=" + status + "&channel=" + chanel + "&pageNo=" + pageNo + "&pageSize=" + pageSize);

        }

        public void ERPPing(PingParam ping) //E-Invoicing system supports calling back to ERP system. To do that, ERP system must be registered to receive the callbacks and must be callable from e-Invoicing. This API is a test API that gets called to verify connectivity, check the credentials provided by taxpayer to e-Invoicing to call it.
        {
            /*
             * Signature Put: /ping
             */

            var request = new HttpRequestMessage(HttpMethod.Put, "Full Path Url")
            {
                Content = JsonContent.Create(ping)
            };

            request.Headers.Add("Content-Type", "application/json");
            request.Headers.Add("Accept-Language", "ar");
            var result = this.requestHttpClient.SendAsync(request);
        }

        public void GetDocumentDetails(string uuid)
        {
            /*
             * Signature: GET /api/v1.0/documents/{uuid}/details
             */
            var result = this.requestHttpClient.GetAsync("/api/v1.0/documents/" + uuid + "/details");


        }

        public void DeclineDocumentCancellation(string uuid)
        {
            /*
             *Signature: PUT /api/v1.0/documents/state/{uuid}/decline/cancelation
             */

            var request = new HttpRequestMessage(HttpMethod.Put, "Full Url");


        }


        public void DeclineDocumentRejection(string uuid)
        {
            /*
             *Signature: PUT /api/v1.0/documents/state/{uuid}/decline/rejection
             */

            var request = new HttpRequestMessage(HttpMethod.Put, "Full Url");


        }
        /////////////////////////////////////////Edit by mohamed kandil //////////////////////
        //public void CreateESGCode(List<ESGItem> newItems) //Create EGS Code Usage API is a way for taxpayer to register his own internal codes in the eInvicing solution.
        //{
        //    for (int i = 0; i < newItems.Count; i++)
        //    {
        //        //Signature: POST /api/v1.0/codetypes/requests/codes

        //        Codes codes = new Codes()
        //        {
        //            items = newItems.ToArray()
        //        };
        //        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1.0/codetypes/requests/codes")
        //        {
        //            Content = JsonContent.Create(codes)
        //        };

        //        var result = requestHttpClient.SendAsync(request);
        //        var response = result.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        //    }

        //}

        public async Task<EtaCreateCodeResponse> CreateESGCodeAsync(List<ESGItem> newItems)
        {
            Codes codes = new Codes
            {
                items = newItems.ToArray()
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "/api/v1.0/codetypes/requests/codes")
            {
                Content = JsonContent.Create(codes)
            };

            var response = await requestHttpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            return new EtaCreateCodeResponse
            {
                IsSuccess = response.IsSuccessStatusCode,
                RawResponse = content,
                StatusCode = response.StatusCode
            };
        }

        public void SearchESGCode() //Search my EGS code usage requests API is responsible for retrieving list of requests that were submitted to the solution by the taxpayer.
        {
            /*
             * Signature: GET /api/v1.0/codetypes/requests/my
             */
            var result = this.requestHttpClient.GetAsync("/api/v1.0/codetypes/requests/my");


        }

        public void RequestCodeReuse(List<ReuseItem> reuseItems) //Taxpayer can pick any code that exist in the solution and request a reuse for this code, which means once code reuse is in place & approved then, taxpayer can use this code in document’s submission.
        {
            /*
             * Signature: PUT api/v1.0/codetypes/requests/codeusages
             */

            var request = new HttpRequestMessage(HttpMethod.Post, "")
            {
                Content = JsonContent.Create(reuseItems)
            };

            var result = requestHttpClient.SendAsync(request);

        }

        public void SearchPublishedCodes(string codeType)
        {
            /*
             * Signature: GET /api/v1.0/codetypes/{codeType}/codes
             */
            var result = this.requestHttpClient.GetAsync("/api/v1.0/codetypes/" + codeType + "/codes");


        }


        public void GetCodeDetailsByItemCode(string codeType, string itemCode)
        {

            /*
             * Signature: GET /api/v1.0/codetypes/{codeType}/codes/{itemCode}
             */
            var result = this.requestHttpClient.GetAsync("/api/v1.0/codetypes/" + codeType + "/codes/" + itemCode);

        }

        public void UpdateESGCodeUsage(long codeUsageRequestId, UpdateItemCodeUsage updateItemCodeUsage) //Update EGS Code Usage API is a way for taxpayer to Update his previous code usage requests as long they are under ‘Submitted’ state
        {
            /*
             * Signature: PUT /api/v1.0/codetypes/requests/codes/{codeUsageRequestId}
             */
            var request = new HttpRequestMessage(HttpMethod.Put, "Full URL Here")
            {
                Content = JsonContent.Create(updateItemCodeUsage)
            };
            var result = this.requestHttpClient.SendAsync(request);
        }

        public void UpdateCode()
        {
        }
        public async Task<object> GetRecentDocumentsAsync(int pageNo, int pageSize)
        {
            var url = $"/api/v1.0/documents/recent?pageNo={pageNo}&pageSize={pageSize}";

            var response = await requestHttpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<object>(json);
        }

    }
}
