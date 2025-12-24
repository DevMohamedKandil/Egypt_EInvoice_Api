using Egypt_EInvoice_Api.BLL;
using Egypt_EInvoice_Api.EInvoiceModel;
using Egypt_EInvoice_Api.Models;
using Egypt_EInvoice_Api.Repos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Egypt_EInvoice_Api.Response;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Egypt_EInvoice_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EInvoicesController : ControllerBase
    {
        private readonly IBaseRepos<VWEInvoice> eInvoiceRepos;
        private readonly IBaseRepos<VwEInvoiceMaster> eInvoiceMasterRepos;

        private readonly IBaseRepos<Bill> billRep;

        private readonly IConfiguration Configuration;

        private readonly EInvoiceGovManager _eta;

        //static Uri IdentityServiceUri = new Uri("https://id.preprod.eta.gov.eg/connect/token");
        //static Uri APISystemUri = new Uri("https://api.preprod.invoicing.eta.gov.eg/api/v1.0/documentsubmissions");


        // static Uri IdentityServiceUri = new Uri("https://id.eta.gov.eg/connect/token");
        // static Uri APISystemUri = new Uri("https://api.invoicing.eta.gov.eg/api/v1.0/documentsubmissions");


        public EInvoicesController(IBaseRepos<VWEInvoice> eInvoiceRepos,
            IBaseRepos<VwEInvoiceMaster> eInvoiceMasterRepos,
             IBaseRepos<Bill> billRep
             , IConfiguration configuration,
             EInvoiceGovManager eta

            )
        {
            this.eInvoiceRepos = eInvoiceRepos;
            this.eInvoiceMasterRepos = eInvoiceMasterRepos;
            this.billRep = billRep;
            Configuration = configuration;
            _eta = eta;


            //this.invoiceLineRepos = invoiceLineRepos;

        }

        [HttpGet]
        [Route("GetUnUploadedInvoice")]
        public List<VwEInvoiceMaster> GetAllUnUploadedInvoice()
        {
            return this.eInvoiceMasterRepos.GetAll().Where(x => x.IsUploaded == null || x.IsUploaded == false).ToList();
        }
        [HttpGet]
        [Route("GetUnUploadedInvoices")]
        public IActionResult GetAllUnUploadedInvoices(
    [FromQuery(Name = "billType")] Guid? billType = null,
    [FromQuery(Name = "DateFrom")] DateTime? dateFrom = null,
    [FromQuery(Name = "DateTo")] DateTime? dateTo = null)
        {
            try
            {
                var query = this.eInvoiceMasterRepos.GetAll()?.AsQueryable()
                            ?? Enumerable.Empty<VwEInvoiceMaster>().AsQueryable();

                // Filter by IsUploaded
                query = query.Where(x => x.IsUploaded == null || x.IsUploaded == false);

                // Optional BillType filter
                if (billType.HasValue)
                    query = query.Where(x => x.TypeGuid == billType.Value);

                // Optional date range filter
                if (dateFrom.HasValue && dateTo.HasValue)
                    query = query.Where(x => x.Date.HasValue && x.Date.Value >= dateFrom.Value && x.Date.Value <= dateTo.Value);

                return Ok(query.ToList());
            }
            catch (Exception ex)
            {
                // ترجع رسالة الخطأ مع كود 400 أو 500 حسب رغبتك
                return BadRequest(new { Message = "حدث خطأ أثناء تنفيذ العملية", Error = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetUploadedInvoices")]
        public List<VwEInvoiceMaster> GetUploadedInvoices()
        {
            return this.eInvoiceMasterRepos.GetAll().Where(x => x.IsUploaded == true).ToList();
        }
        [HttpGet]
        [Route("GetAllInvoices")]
        public List<VWEInvoice> GetAll()
        {
            return this.eInvoiceRepos.GetAll();
        }


        [HttpPost]
        [Route("CreateESGCode")]
        public void CreateESGCode()

        {
            EInvoiceGovManager obj = new EInvoiceGovManager();
            var loginResponse = obj.Login();
            if (loginResponse != null)
            {

                // obj.CreateESGCode();
                //  return result;
            }
            // return "";
        }


        [HttpPost]
        [Route("UploadInvoice")]
        public BillResponse UploadInvoice(VwEInvoiceMasterdto bill)

        {
            IConfiguration config = new ConfigurationBuilder()
  .AddJsonFile("appsettings.json")
  .AddEnvironmentVariables()
  .Build();


            Appsettings settings = config.GetRequiredSection("Settings").Get<Appsettings>();
            // string ErrorMessage = string.Empty;
            var item = this.eInvoiceMasterRepos.FindByGuid(Guid.Parse(bill.InternalId));

            int tempint = 0;

            //item.ErrorMessage = "";

            EInvoiceModel.Document obj = new EInvoiceModel.Document();
            List<EInvoiceModel._documents> List = new List<EInvoiceModel._documents>();
            EInvoiceGovManager EGovmanager = new EInvoiceGovManager();

            EInvoiceModel.Issuer issuer = new EInvoiceModel.Issuer();

            EInvoiceModel.Address Address = new EInvoiceModel.Address();

            EInvoiceModel.Receiver Receiver = new EInvoiceModel.Receiver();

            EInvoiceModel.Payment Payment = new EInvoiceModel.Payment();


            issuer.name = item.IssuerName;
            issuer.id = item.IssuerId;

            if (string.IsNullOrEmpty(item.IssuerId))
            {
                // item.ErrorMessage = "Issuer Id  is required";
                return new BillResponse()
                {
                    BillNo = item.BillNo,
                    Msg = "Issuer Tax Number  is required",
                    BillGuid = item.InternalId
                };

            }
            else
            {
                if (item.IssuerType == "B")
                {
                    int numbers = int.Parse(item.IssuerId.Length.ToString());

                    if (numbers < 9 || numbers > 9)
                    {
                        return
                   new BillResponse()
                   {
                       BillNo = item.BillNo,
                       Msg = "Issuer Tax Number should be 9 digits",
                       BillGuid = item.InternalId
                   };

                    }
                }

                if (item.IssuerType == "P")
                {
                    int numbers = int.Parse(item.IssuerId.Length.ToString());

                    if (numbers < 14 || numbers > 14)
                    {
                        return
                   new BillResponse()
                   {
                       BillNo = item.BillNo,
                       Msg = "Issuer National Id should be 14 digits",
                       BillGuid = item.InternalId
                   };

                    }
                }
            }
            if (string.IsNullOrEmpty(item.ReceiverId))
            {
               
                return
                    new BillResponse()
                    {
                        BillNo = item.BillNo,
                        Msg = "Receiver Tax Number is required",
                        BillGuid = item.InternalId
                    };
               


            }
            else
            {
                if (item.ReceiverType == "B")
                {
                    int numbers = int.Parse(item.ReceiverId.Length.ToString());
                    
                    if (numbers < 9 || numbers > 9)
                    {
                        return
                   new BillResponse()
                   {
                       BillNo = item.BillNo,
                       Msg = "Receiver Tax Number should be 9 digits",
                       BillGuid = item.InternalId
                   };

                    }
                }
                if (item.ReceiverType == "P")
                {
                    int numbers = int.Parse(item.ReceiverId.Length.ToString());

                    if (numbers < 14 || numbers > 14)
                    {
                        return
                   new BillResponse()
                   {
                       BillNo = item.BillNo,
                       Msg = "Receiver National Id should be 14 digits",
                       BillGuid = item.InternalId
                   };

                    }
                }
            }
            //
            if (string.IsNullOrEmpty(item.IssuerCountryCoder))
            {
                return new BillResponse()
                {
                    BillNo = item.BillNo,
                    Msg = "Issuer Country Code is Required",
                    BillGuid = item.InternalId
                };
            }

            if (string.IsNullOrEmpty(item.IssuerGovernate))
            {
                return new BillResponse()
                {
                    BillNo = item.BillNo,
                    Msg = "Issuer Governorate Code is Required",
                    BillGuid = item.InternalId
                };
            }

            if (string.IsNullOrEmpty(item.IssuerRegionCity))
            {
                return new BillResponse()
                {
                    BillNo = item.BillNo,
                    Msg = "Issuer City is Required",
                    BillGuid = item.InternalId
                };
            }

            if (string.IsNullOrEmpty(item.IssuerStreet))
            {
                return new BillResponse()
                {
                    BillNo = item.BillNo,
                    Msg = "Issuer Street is Required",
                    BillGuid = item.InternalId
                };
            }

            if (string.IsNullOrEmpty(item.IssuerBuildingNumber))
            {
                return new BillResponse()
                {
                    BillNo = item.BillNo,
                    Msg = "Issuer Building No is Required",
                    BillGuid = item.InternalId
                };
            }

            //
            if (string.IsNullOrEmpty(item.ReceiverCountryCode))
            {
                return new BillResponse()
                {
                    BillNo = item.BillNo,
                    Msg = "Receiver Country Code is Required",
                    BillGuid = item.InternalId
                };
            }

            if (string.IsNullOrEmpty(item.ReceiverGovernate))
            {
                return new BillResponse()
                {
                    BillNo = item.BillNo,
                    Msg = "Receiver Governorate Code is Required",
                    BillGuid = item.InternalId
                };
            }

            if (string.IsNullOrEmpty(item.ReceiverRegionCity))
            {
                return new BillResponse()
                {
                    BillNo = item.BillNo,
                    Msg = "Receiver City is Required",
                    BillGuid = item.InternalId
                };
            }

            if (string.IsNullOrEmpty(item.ReceiverStreet))
            {
                return new BillResponse()
                {
                    BillNo = item.BillNo,
                    Msg = "Receiver Street is Required",
                    BillGuid = item.InternalId
                };
            }

            if (string.IsNullOrEmpty(item.ReceiverBuildingNumber))
            {
                return new BillResponse()
                {
                    BillNo = item.BillNo,
                    Msg = "Receiver Building No is Required",
                    BillGuid = item.InternalId
                };
            }
            //

            if (string.IsNullOrEmpty(item.branchId) || int.TryParse(item.branchId, out tempint) == false)
            {
                return new BillResponse()
                {
                    BillNo = item.BillNo,
                    Msg = "branch Id  is required (number)",
                    BillGuid = item.InternalId
                };

            }


            Address = new EInvoiceModel.Address();

            Address.governate = item.IssuerGovernate;
            Address.regionCity = item.IssuerRegionCity;
            Address.street = item.IssuerStreet;
            Address.buildingNumber = item.IssuerBuildingNumber;
            Address.country = item.IssuerCountryCoder;
            Address.floor = item.IssuerFloorNo;
            Address.landmark = item.IssuerLandMark;
            Address.room = item.IssuerRoom;
            Address.postalCode = item.IssuerPostalCode;
            Address.branchId = item.branchId;
            Address.additionalInformation = item.IssuerAdditionalInformation;

            issuer.address = Address;
            issuer.type = item.IssuerType;

            obj.issuer = issuer;
            Receiver.id = item.ReceiverId;

            Receiver.name = item.ReceiverName;
            Receiver.type = item.ReceiverType == null ? "P" : item.ReceiverType;


            Address = new EInvoiceModel.Address();

            Address.governate = item.ReceiverGovernate;
            Address.regionCity = item.ReceiverRegionCity;
            Address.street = item.ReceiverStreet;
            Address.buildingNumber = item.ReceiverBuildingNumber;
            Address.country = item.ReceiverCountryCode;
            Address.floor = item.ReceiverFloorNo;
            Address.landmark = item.ReceiverLandMark;
            Address.room = item.ReceiverRoom;
            Address.postalCode = item.ReceiverPostalCode;
            
            Address.branchId = item.branchId;
            
            Address.additionalInformation = item.ReceiverAdditionalInformation;
            Receiver.address = Address;

            obj.receiver = Receiver;

            obj.documentType = item.DocumentType;
            obj.documentTypeVersion = item.DocumentTypeVersion;
            DateTime correctDate = item.DateTimeIssued.AddHours(-2);
            obj.dateTimeIssued = correctDate.ToString("yyyy-MM-dd") + "T" + correctDate.ToString("HH:mm:ss") + "Z";
            //"2022-02-17T23:59:59Z";

            obj.taxpayerActivityCode = item.ActivityCode;

           
            obj.internalID = (settings.InvoiceTitle == 1)?item.BillNo:item.InternalId;
            

            if (string.IsNullOrEmpty(item.InternalId))
            {
                return new BillResponse()
                {
                    BillNo = item.BillNo,
                    Msg = "Internal ID is required",
                    BillGuid = item.InternalId
                };

            }

            obj.purchaseOrderReference = item.PurchaseOrderReference;
            obj.purchaseOrderDescription = item.PurchaseOrderDescription;
            obj.salesOrderReference = item.SalesOrderReference;
            obj.salesOrderDescription = item.SalesOrderDescription;
            obj.proformaInvoiceNumber = item.ProformaInvoiceNumber;

            obj.payment = new EInvoiceModel.Payment();

            obj.payment.bankAccountIBAN = item.PaymentBankAccountIBAN;
            obj.payment.bankAccountNo = item.PaymentBankAccountNo;
            obj.payment.bankAddress = item.PaymentBankAddress;
            obj.payment.bankName = item.PaymentBankName;
            obj.payment.swiftCode = item.PaymentSwiftCode;
            obj.payment.terms = item.PaymentTerms;

            obj.delivery = new EInvoiceModel.Delivery();

            obj.delivery.exportPort = item.DeliveryExportPort;
            obj.delivery.approach = item.DeliveryApproch;
            obj.delivery.countryOfOrigin = item.DeliveryCountryOfOrigin;
            
            obj.delivery.dateValidity = item.DeliveryDateValidity == null ? "" : item.DeliveryDateValidity;

            obj.delivery.grossWeight = 0;
            obj.delivery.netWeight = 0;
            obj.delivery.packaging = item.DeliveryPackaging;
            obj.delivery.terms = item.DeliveryTerms;



            EInvoiceModel.InvoiceLine invoiceLine = new EInvoiceModel.InvoiceLine();
            List<VWInvoiceLine> invoicesitems = new List<VWInvoiceLine>();

            if (!string.IsNullOrEmpty(item.InternalId))
            {
                BillItemsRepos invoiceLineRepos = new BillItemsRepos();

                invoicesitems = invoiceLineRepos.SearchByGuid(Guid.Parse(item.InternalId));
                var index = 0;
                obj.invoiceLines = new InvoiceLine[invoicesitems.Count];


                foreach (var item2 in invoicesitems)
                {
                    if (item2.itemCode == null || item2.itemCode == "")
                    {
                        return new BillResponse()
                        {
                            Msg = "item has not code for  :" + item2.internalCode,
                            BillNo = item.BillNo,
                            BillGuid = item.InternalId
                        };
                    }
                    invoiceLine = new EInvoiceModel.InvoiceLine();
                    invoiceLine.description = item2.description;
                    invoiceLine.discount = new EInvoiceModel.Discount();
                    if (item2.discAmount == 0)
                    {
                        invoiceLine.discount.amount = 0;
                    }
                    else
                    {
                        invoiceLine.discount.amount = Math.Round((decimal)item2.discAmount, 5);
                    }
                    if (item2.discRate == 0)
                    { invoiceLine.discount.rate = 0; }
                    else
                    {
                        invoiceLine.discount.rate = Math.Round((decimal)item2.discRate, 5);
                    }
                    invoiceLine.internalCode = item2.internalCode;
                    invoiceLine.itemCode = item2.itemCode;
                    if (item2.itemsDiscount == 0)
                    {
                        invoiceLine.itemsDiscount = 0;
                    }
                    else
                    {
                        invoiceLine.itemsDiscount = Math.Round((decimal)item2.itemsDiscount, 5);
                    }
                    invoiceLine.itemType = item2.itemType;
                    
                    invoiceLine.netTotal =  Math.Round((decimal)item2.netTotal,5);
                  
                    invoiceLine.quantity =Math.Round((decimal)item2.quantity,5);
                    invoiceLine.salesTotal = Math.Round((decimal)item2.salesTotal,5);

                    //////////////////////edit by kandil /////////////////////////
                    // invoiceLine.total = Math.Round((decimal)item2.total,5);

                    invoiceLine.total = Math.Round((decimal)item2.salesTotal + (decimal)item2.AddTax, 5); // Fixed: total = sales + tax

                    if (item2.totalTaxableFees == 0)
                    {
                        invoiceLine.totalTaxableFees = 0;
                    }
                    else
                    {
                        invoiceLine.totalTaxableFees = (decimal)item2.totalTaxableFees;
                    }
                    invoiceLine.unitType = "EA";
                  
                    if (item2.TaxPercent == 0)
                    {
                        invoiceLine.taxableItems = new TaxableItem[1];
                        invoiceLine.taxableItems[0] = new TaxableItem()
                        {
                            taxType = "T1",
                            amount = 0,
                            subType = "V004",
                            rate = 0


                        };

                    }
                    else
                    {
                        invoiceLine.taxableItems = new TaxableItem[1];
                        invoiceLine.taxableItems[0] = new TaxableItem()
                        {
                            taxType = "T1",
                            amount =(decimal)item2.AddTax,
                            subType = "V009",
                            rate = (decimal)item2.TaxPercent 


                        };
                 

                    }


                    //invoiceLine.unitValue = new EInvoiceModel.UnitValue();

                    //invoiceLine.unitValue.amountEGP =Math.Round( decimal.Parse(item2.amountEGP.ToString()),5);

                    //invoiceLine.unitValue.currencySold = "EGP";
                    /////////////////////////////////Edited Currency Code ////////////////////////////////////

                    invoiceLine.unitValue = new EInvoiceModel.UnitValue();
                    invoiceLine.unitValue.currencySold = (item2.currencySold ?? "");
                    invoiceLine.unitValue.amountEGP = Math.Round((decimal)item2.amountEGP, 5);
                    invoiceLine.unitValue.amountSold = 0;
                    if (invoiceLine.unitValue.currencyNumber != 1 && item2.amountSold > 0)
                    {
                        invoiceLine.unitValue.amountSold = Math.Round((decimal)item2.amountSold, 5);
                        invoiceLine.unitValue.currencyExchangeRate = Math.Round((decimal)item2.currencyExchangeRate, 5);
                    }
                    //////////////////////////////////////////////////////////////////////////////////////////



                    invoiceLine.valueDifference = (decimal)item2.valueDifference;
                    
                    obj.invoiceLines[index] = invoiceLine;
                    

                    index++;

                 

                }

            }

            obj.totalSalesAmount = item.TotalSalesAmount.HasValue ? Math.Round(decimal.Parse(item.TotalSalesAmount.Value.ToString()), 5) : 0;

            obj.totalDiscountAmount = item.TotalDiscountAmount.HasValue ? Math.Round(decimal.Parse(item.TotalDiscountAmount.Value.ToString()), 5) : 0;

            obj.netAmount = item.NetAmount.HasValue ? Math.Round(decimal.Parse(item.NetAmount.Value.ToString()), 5) : 0;

            obj.extraDiscountAmount = item.ExtraDiscountAmount.HasValue ? Math.Round((decimal)item.ExtraDiscountAmount.Value, 5) : 0;
            obj.totalItemsDiscountAmount = item.TotalItemsDiscountAmount.HasValue ? Math.Round((decimal)item.TotalItemsDiscountAmount.Value, 5) : 0;
            //
            // obj.totalAmount = item.TotalAmount.HasValue ? Math.Round(decimal.Parse(item.TotalAmount.Value.ToString()), 5) : 0;   
            //
            // الحل الصحيح لما يكون double? (مش decimal?)
            
            obj.totalAmount = Math.Round((decimal)((item.TotalSalesAmount ?? 0d) + (item.AddTax ?? 0d)), 5);
            obj.taxTotals = new TaxTotal[1];
            obj.taxTotals[0] = new TaxTotal
            {
                taxType = "T1",
                amount =(decimal)item.AddTax

            };
            
            string output2 = JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                FloatFormatHandling = FloatFormatHandling.String,
                FloatParseHandling = FloatParseHandling.Decimal,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = DateParseHandling.None
            });
          


            SaveInvoice(output2, item.InternalId);

            BillController obj3 = new BillController(billRep);
            obj3.Update(Guid.Parse(item.InternalId));
            return new BillResponse()
            {
                BillNo = item.BillNo,
                Msg = "Invoice Uploaded successfully",
                BillGuid = item.InternalId
            };


            ////
            ///

            #region -- comment

            //TokenSigner token = new TokenSigner();
            //var tokenSinger = token.SignWithCMS(token.Serialize(JObject.Parse(output2)));
            //// obj.signatures = new Signature() { type = "I", value = tokenSinger };

            //obj.signatures = new Signature[1];
            //obj.signatures[0] = new Signature
            //{
            //    signatureType = "I",
            //    value = tokenSinger

            //};

            //string output3 = JsonConvert.SerializeObject(obj);


            /////

            //string StingInvoice = "{'documents': [" + output3 + "]}";
            //JObject JSONInvoice = JObject.Parse(StingInvoice);
            //var content = new StringContent(JSONInvoice.ToString(), Encoding.UTF8, "application/json");
            ////var loginResponse = EGovmanager.Login();
            //try
            //{
            //    var result = Task.Run(() => PostURIWithToken(APISystemUri, content, GetAccessToken())).GetAwaiter().GetResult();

            //    //var  = t;
            //    if (result != null)
            //    {

            //        if (result != null)
            //        {
            //            if (result.submissionUUID != null)
            //            {
            //                BillController obj3 = new BillController(billRep);
            //                obj3.Update(Guid.Parse(item.InternalId));
            //                return new BillResponse()
            //                {
            //                    BillNo = item.BillNo,
            //                    Msg = "Invoice Uploaded successfully",
            //                    BillGuid = item.InternalId
            //                };

            //            }
            //            else
            //            {
            //                if (result.acceptedDocuments != null)
            //                {
            //                    if (result.acceptedDocuments.Count > 0)
            //                    {
            //                        BillController obj3 = new BillController(billRep);
            //                        obj3.Update(Guid.Parse(item.InternalId));
            //                        return new BillResponse()
            //                        {
            //                            BillNo = item.BillNo,
            //                            Msg = "Invoice Uploaded successfully",
            //                            BillGuid = item.InternalId
            //                        };
            //                    }
            //                    else
            //                    {
            //                        string err = "";
            //                        if (result.rejectedDocuments.Count > 0)
            //                        {
            //                            foreach (var res in result.rejectedDocuments)
            //                            {
            //                                if (res.error.details != null)
            //                                {
            //                                    err += "An error occured at bill " + item.BillNo + ": " + res.error.details[0].message + "\n";
            //                                }
            //                                else
            //                                {
            //                                    err += "An error occured at bill " + item.BillNo + ": " + res.error.message + "\n";
            //                                }

            //                            }

            //                        }
            //                        return new BillResponse()
            //                        {
            //                            BillNo = item.BillNo,
            //                            Msg = err,
            //                            BillGuid = item.InternalId
            //                        };
            //                    }

            //                }
            //                else
            //                {
            //                    string err = "";
            //                    if (result.rejectedDocuments.Count > 0)
            //                    {
            //                        foreach (var res in result.rejectedDocuments)
            //                        {
            //                            if (res.error.details != null)
            //                            {
            //                                err += "An error occured at bill " + item.BillNo + ": " + res.error.details[0].message + "\n";
            //                            }
            //                            else
            //                            {
            //                                err += "An error occured at bill " + item.BillNo + ": " + res.error.message + "\n";
            //                            }

            //                        }

            //                    }
            //                    return new BillResponse()
            //                    {
            //                        BillNo = item.BillNo,
            //                        Msg = err,
            //                        BillGuid = item.InternalId
            //                    };
            //                }

            //            }





            //        }
            //        else
            //        {
            //            return new BillResponse()
            //            {
            //                BillNo = item.BillNo,
            //                Msg = "Upload process failed, Please try again later",
            //                BillGuid = item.InternalId
            //            };

            //        }

            //        //BillController obj3 = new BillController(billRep);
            //        //obj3.Update(Guid.Parse(item.InternalId));

            //    }
            //    else
            //    {
            //        return new BillResponse()
            //        {
            //            BillNo = item.BillNo,
            //            Msg = "Upload process failed, Please try again later",
            //            BillGuid = item.InternalId
            //        };

            //    }



            //}
            //catch (Exception ex)
            //{

            //    return new BillResponse()
            //    {
            //        BillNo = item.BillNo,
            //        Msg = ex.Message,
            //        BillGuid = item.InternalId
            //    };
            //}
            #endregion

        }


        public void SaveInvoice(string strinvoice,string filename)
        {
            IConfiguration config = new ConfigurationBuilder()
.AddJsonFile("appsettings.json")
.AddEnvironmentVariables()
.Build();


            Appsettings settings = config.GetRequiredSection("Settings").Get<Appsettings>();
            string path = @"C:\Invoices";
            if (settings.InvoiceFolderPath != null && !string.IsNullOrEmpty(settings.InvoiceFolderPath))
                path = settings.InvoiceFolderPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);

               
            }
            //string Invoice = strinvoice.Replace("\"signatures\":null,", "")
            //.Replace(":null", ":\"\"")
            //.Replace("null", "\"\"");
            string filepath = path + "\\" + filename + ".json";

            System.IO.File.WriteAllBytes(filepath, System.Text.Encoding.UTF8.GetBytes(strinvoice));

            //if (!System.IO.File.Exists(filepath))
            //{
            //    // Create a file to write to.   
            //    using (StreamWriter sw = System.IO.File.CreateText(filepath))
            //    {
            //        sw.WriteLine(Invoice);
            //    }
            //}
            //else
            //{
            //    using (StreamWriter sw = System.IO.File.AppendText(filepath))
            //    {
            //        sw.WriteLine(Invoice);
            //    }
            //}
        }
        //static async Task<DocumetSubmitResponse> PostURIWithToken(Uri u, HttpContent c, string t)
        //{
        //    DocumetSubmitResponse response = new DocumetSubmitResponse();
        //    string responseString = "";
        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", t);
        //        HttpResponseMessage result = await client.PostAsync(u, c);
        //        responseString = await result.Content.ReadAsStringAsync();
        //        response = JsonConvert.DeserializeObject<DocumetSubmitResponse>(responseString);

        //    }
        //    if (response.acceptedDocuments == null && response.rejectedDocuments == null && response.submissionUUID == null)
        //    {
        //        List<RejectedDocument> rejectedDocuments = new List<RejectedDocument>();
        //        rejectedDocuments.Add(new RejectedDocument()
        //        {
        //            error = new Error()
        //            {
        //                message = responseString
        //            }
        //        });
        //        return new DocumetSubmitResponse()
        //        {
        //            rejectedDocuments = rejectedDocuments
        //        };
        //    }
        //    else if (response.rejectedDocuments.Count > 0) 
        //    {
        //        List<RejectedDocument> rejectedDocuments = response.rejectedDocuments;
               
        //        return new DocumetSubmitResponse()
        //        {
        //            rejectedDocuments = rejectedDocuments
        //        };
        //    }
            
        //    return response;
        //}

        //static string GetAccessToken()
        //{
        //    System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        //    var payload = "Client_id=" + Helper.client_id + "&Client_secret=" + Helper.client_secret + "&grant_type=client_credentials";
        //    HttpContent c = new StringContent(payload, Encoding.UTF8, "application/x-www-form-urlencoded");
        //    var t = Task.Run(() => PostURI(IdentityServiceUri, c));
        //    t.Wait();
        //    return JObject.Parse(t.Result)["access_token"].ToString();
        //}
        //static async Task<string> PostURI(Uri u, HttpContent c)
        //{
        //    var response = string.Empty;
        //    using (var client = new HttpClient())
        //    {
        //        HttpResponseMessage result = await client.PostAsync(u, c);
        //        if (result.IsSuccessStatusCode)
        //        {
        //            response = await result.Content.ReadAsStringAsync();
        //        }
        //    }
        //    return response;
        //}

        [HttpPost]
        [Route("UploadInvoices")]
        public List<BillResponse> UploadInvoices(List<VwEInvoiceMasterdto> itemList)

        {
           
            List<BillResponse> uploadeditems = new List<BillResponse>();
            VwEInvoiceMasterdto obj = new VwEInvoiceMasterdto();
            obj.ErrorMessage = "";
            foreach (var item in itemList)
            {
                uploadeditems.Add(UploadInvoice(item));

            }

            return uploadeditems;


        }
    

         
            [HttpGet("GetRecentDocuments")]
            public async Task<IActionResult> GetRecentDocuments(
                int pageNo = 1,
                int pageSize = 20)
            {
                var result = await _eta.GetRecentDocumentsAsync(pageNo, pageSize);
                return Ok(result);
            }
        


    }
}
