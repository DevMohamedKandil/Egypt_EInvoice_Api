using Egypt_EInvoice_Api.BLL;
using Egypt_EInvoice_Api.EInvoiceModel;
using Egypt_EInvoice_Api.Models;
using Egypt_EInvoice_Api.Repos;
using Egypt_EInvoice_Api.Services;
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
using Microsoft.Extensions.Logging;

namespace Egypt_EInvoice_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EInvoicesController : ControllerBase
    {
        private readonly IBaseRepos<VWEInvoice> eInvoiceRepos;
        private readonly IBaseRepos<VwEInvoiceMaster> eInvoiceMasterRepos;
        private readonly IBaseRepos<VWInvoiceLine> invoiceLineRepos;

        private readonly IConfiguration Configuration;

        private readonly EInvoiceGovManager _eta;
        private readonly IInvoiceSigningService invoiceSigningService;
        private readonly IEtaSubmissionService etaSubmissionService;
        private readonly IBillUploadStatusService billUploadStatusService;
        private readonly ILogger<EInvoicesController> logger;

        public EInvoicesController(
            IBaseRepos<VWEInvoice> eInvoiceRepos,
            IBaseRepos<VwEInvoiceMaster> eInvoiceMasterRepos,
            IBaseRepos<VWInvoiceLine> invoiceLineRepos,
            IConfiguration configuration,
            EInvoiceGovManager eta,
            IInvoiceSigningService invoiceSigningService,
            IEtaSubmissionService etaSubmissionService,
            IBillUploadStatusService billUploadStatusService,
            ILogger<EInvoicesController> logger
            )
        {
            this.eInvoiceRepos = eInvoiceRepos;
            this.eInvoiceMasterRepos = eInvoiceMasterRepos;
            this.invoiceLineRepos = invoiceLineRepos;
            Configuration = configuration;
            _eta = eta;
            this.invoiceSigningService = invoiceSigningService;
            this.etaSubmissionService = etaSubmissionService;
            this.billUploadStatusService = billUploadStatusService;
            this.logger = logger;
        }

        [HttpGet]
        [Route("GetUnUploadedInvoice")]
        public List<VwEInvoiceMaster> GetAllUnUploadedInvoice()
        {
            try
            {
                return this.eInvoiceMasterRepos.GetAll()
                    .Where(x => x.IsUploaded == null || x.IsUploaded == false)
                    .ToList();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to get un-uploaded invoices");
                return new List<VwEInvoiceMaster>();
            }
        }

        [HttpGet]
        [Route("GetUnUploadedInvoices")]
        public IActionResult GetAllUnUploadedInvoices(
            [FromQuery(Name = "BillType")] Guid? billType = null,
            [FromQuery(Name = "DateFrom")] DateTime? dateFrom = null,
            [FromQuery(Name = "DateTo")] DateTime? dateTo = null)
        {
            try
            {
                var query = this.eInvoiceMasterRepos.GetAll()?.AsQueryable()
                            ?? Enumerable.Empty<VwEInvoiceMaster>().AsQueryable();

                query = query.Where(x => x.IsUploaded == null || x.IsUploaded == false);

                if (billType.HasValue)
                    query = query.Where(x => x.TypeGuid == billType.Value);

                if (dateFrom.HasValue && dateTo.HasValue)
                    query = query.Where(x => x.Date.HasValue && x.Date.Value >= dateFrom.Value && x.Date.Value <= dateTo.Value);

                return Ok(query.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "حدث خطأ أثناء تنفيذ العملية", Error = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetUploadedInvoices")]
        public List<VwEInvoiceMaster> GetUploadedInvoices()
        {
            try
            {
                return this.eInvoiceMasterRepos.GetAll()
                    .Where(x => x.IsUploaded == true)
                    .ToList();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to get uploaded invoices");
                return new List<VwEInvoiceMaster>();
            }
        }

        [HttpGet]
        [Route("GetAllInvoices")]
        public List<VWEInvoice> GetAll()
        {
            try
            {
                return this.eInvoiceRepos.GetAll();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to get all invoices");
                return new List<VWEInvoice>();
            }
        }

        [HttpPost]
        [Route("CreateESGCode")]
        public void CreateESGCode()
        {
            var loginResponse = _eta.Login();
            if (loginResponse != null)
            {
                // obj.CreateESGCode();
            }
        }

        [HttpPost]
        [Route("UploadInvoice")]
        public BillResponse UploadInvoice(VwEInvoiceMasterdto bill)
        {
            Appsettings settings = Configuration.GetRequiredSection("Settings").Get<Appsettings>();

            var item = this.eInvoiceMasterRepos.FindByGuid(Guid.Parse(bill.InternalId));

            int tempint = 0;

            EInvoiceModel.Document obj = new EInvoiceModel.Document();
            List<EInvoiceModel._documents> List = new List<EInvoiceModel._documents>();

            EInvoiceModel.Issuer issuer = new EInvoiceModel.Issuer();
            EInvoiceModel.Address Address = new EInvoiceModel.Address();
            EInvoiceModel.Receiver Receiver = new EInvoiceModel.Receiver();
            EInvoiceModel.Payment Payment = new EInvoiceModel.Payment();

            issuer.name = item.IssuerName;
            issuer.id = item.IssuerId;

            // ===================== Issuer Validations =====================

            if (string.IsNullOrEmpty(item.IssuerId))
            {
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
                    if (item.IssuerId.Length != 9)
                    {
                        return new BillResponse()
                        {
                            BillNo = item.BillNo,
                            Msg = "Issuer Tax Number should be 9 digits",
                            BillGuid = item.InternalId
                        };
                    }
                }

                if (item.IssuerType == "P")
                {
                    if (item.IssuerId.Length != 14)
                    {
                        return new BillResponse()
                        {
                            BillNo = item.BillNo,
                            Msg = "Issuer National Id should be 14 digits",
                            BillGuid = item.InternalId
                        };
                    }
                }
            }

            // ===================== Receiver Validations =====================

            if (string.IsNullOrEmpty(item.ReceiverId))
            {
                return new BillResponse()
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
                    if (item.ReceiverId.Length != 9)
                    {
                        return new BillResponse()
                        {
                            BillNo = item.BillNo,
                            Msg = "Receiver Tax Number should be 9 digits",
                            BillGuid = item.InternalId
                        };
                    }
                }

                if (item.ReceiverType == "P")
                {
                    if (item.ReceiverId.Length != 14)
                    {
                        return new BillResponse()
                        {
                            BillNo = item.BillNo,
                            Msg = "Receiver National Id should be 14 digits",
                            BillGuid = item.InternalId
                        };
                    }
                }
            }

            // ===================== Issuer Address Validations =====================

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

            // ===================== Receiver Address Validations =====================

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

            // ===================== Branch Validation =====================

            if (string.IsNullOrEmpty(item.branchId) || int.TryParse(item.branchId, out tempint) == false)
            {
                return new BillResponse()
                {
                    BillNo = item.BillNo,
                    Msg = "branch Id  is required (number)",
                    BillGuid = item.InternalId
                };
            }

            // ===================== Build Issuer Address =====================

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

            // ===================== Build Receiver =====================

            Receiver.id = item.ReceiverId;
            Receiver.name = item.ReceiverName;
            Receiver.type = item.ReceiverType ?? "P";

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

            // ===================== Build Document Header =====================

            obj.documentType = item.DocumentType;
            obj.documentTypeVersion = item.DocumentTypeVersion;

            DateTime correctDate = item.DateTimeIssued?.AddHours(-2) ?? DateTime.UtcNow;
            obj.dateTimeIssued = correctDate.ToString("yyyy-MM-dd") + "T" + correctDate.ToString("HH:mm:ss") + "Z";

            obj.taxpayerActivityCode = item.ActivityCode;
            obj.internalID = (settings.InvoiceTitle == 1) ? item.BillNo : item.InternalId;

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

            // ===================== Payment =====================

            obj.payment = new EInvoiceModel.Payment();
            obj.payment.bankAccountIBAN = item.PaymentBankAccountIBAN;
            obj.payment.bankAccountNo = item.PaymentBankAccountNo;
            obj.payment.bankAddress = item.PaymentBankAddress;
            obj.payment.bankName = item.PaymentBankName;
            obj.payment.swiftCode = item.PaymentSwiftCode;
            obj.payment.terms = item.PaymentTerms;

            // ===================== Delivery =====================

            obj.delivery = new EInvoiceModel.Delivery();
            obj.delivery.exportPort = item.DeliveryExportPort;
            obj.delivery.approach = item.DeliveryApproch;
            obj.delivery.countryOfOrigin = item.DeliveryCountryOfOrigin;
            obj.delivery.dateValidity = item.DeliveryDateValidity ?? "";
            obj.delivery.grossWeight = 0;
            obj.delivery.netWeight = 0;
            obj.delivery.packaging = item.DeliveryPackaging;
            obj.delivery.terms = item.DeliveryTerms;

            // ===================== Invoice Lines =====================

            EInvoiceModel.InvoiceLine invoiceLine = new EInvoiceModel.InvoiceLine();
            List<VWInvoiceLine> invoicesitems = new List<VWInvoiceLine>();

            if (!string.IsNullOrEmpty(item.InternalId))
            {
                invoicesitems = invoiceLineRepos.SearchByGuid(Guid.Parse(item.InternalId));
                var index = 0;
                obj.invoiceLines = new InvoiceLine[invoicesitems.Count];

                foreach (var item2 in invoicesitems)
                {
                    if (string.IsNullOrEmpty(item2.itemCode))
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

                    invoiceLine.discount.amount = item2.discAmount == 0
                        ? 0m
                        : (decimal)Math.Round(item2.discAmount, 5);

                    invoiceLine.discount.rate = item2.discRate == 0
                        ? 0m
                        : (decimal)Math.Round(item2.discRate, 5);

                    invoiceLine.internalCode = item2.internalCode;
                    invoiceLine.itemCode = item2.itemCode;

                    invoiceLine.itemsDiscount = item2.itemsDiscount == 0
                        ? 0d
                        : Math.Round(item2.itemsDiscount, 5);

                    invoiceLine.itemType = item2.itemType;
                    invoiceLine.netTotal = Math.Round(item2.netTotal, 5);
                    invoiceLine.quantity = Math.Round(item2.quantity, 5);
                    invoiceLine.salesTotal = Math.Round(item2.salesTotal, 5);

                    invoiceLine.total = Math.Round(item2.salesTotal + (item2.AddTax ?? 0d), 5);

                    invoiceLine.totalTaxableFees = item2.totalTaxableFees == 0
                        ? 0d
                        : item2.totalTaxableFees;

                    invoiceLine.unitType = "EA";

                    invoiceLine.valueDifference = item2.valueDifference;

                    invoiceLine.taxableItems = new TaxableItem[1];

                    if (item2.TaxPercent == null || item2.TaxPercent == 0d)
                    {
                        invoiceLine.taxableItems[0] = new TaxableItem()
                        {
                            taxType = "T1",
                            amount = 0m,
                            subType = "V004",
                            rate = 0m
                        };
                    }
                    else
                    {
                        invoiceLine.taxableItems[0] = new TaxableItem()
                        {
                            taxType = "T1",
                            amount = (decimal)(item2.AddTax ?? 0d),
                            subType = "V009",
                            rate = (decimal)(item2.TaxPercent ?? 0d)
                        };
                    }

                    invoiceLine.unitValue = new EInvoiceModel.UnitValue();
                    invoiceLine.unitValue.currencySold = item2.currencySold ?? "";
                    invoiceLine.unitValue.amountEGP = (decimal)Math.Round(item2.amountEGP, 5);
                    invoiceLine.unitValue.amountSold = 0m;

                    if (invoiceLine.unitValue.currencyNumber != 1 && item2.amountSold > 0)
                    {
                        invoiceLine.unitValue.amountSold = (decimal)Math.Round(item2.amountSold, 5);
                        invoiceLine.unitValue.currencyExchangeRate = (decimal)Math.Round(item2.currencyExchangeRate, 5);
                    }

                    obj.invoiceLines[index] = invoiceLine;
                    index++;
                }
            }

            // ===================== Document Totals =====================

            obj.totalSalesAmount = item.TotalSalesAmount.HasValue
                ? (decimal)Math.Round(item.TotalSalesAmount.Value, 5)
                : 0m;

            obj.totalDiscountAmount = item.TotalDiscountAmount.HasValue
                ? (decimal)Math.Round(item.TotalDiscountAmount.Value, 5)
                : 0m;

            obj.netAmount = item.NetAmount.HasValue
                ? (decimal)Math.Round(item.NetAmount.Value, 5)
                : 0m;

            obj.extraDiscountAmount = item.ExtraDiscountAmount.HasValue
                ? (decimal)Math.Round(item.ExtraDiscountAmount.Value, 5)
                : 0m;

            obj.totalItemsDiscountAmount = item.TotalItemsDiscountAmount.HasValue
                ? (decimal)Math.Round(item.TotalItemsDiscountAmount.Value, 5)
                : 0m;

            obj.totalAmount = (decimal)Math.Round(
                (item.TotalSalesAmount ?? 0d) + (item.AddTax ?? 0d),
                5);

            obj.taxTotals = new TaxTotal[1];
            obj.taxTotals[0] = new TaxTotal
            {
                taxType = "T1",
                amount = (decimal)(item.AddTax ?? 0d)
            };

            // ===================== Serialize & Save (unsigned - root folder) =====================

            string output2 = JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                FloatFormatHandling = FloatFormatHandling.String,
                FloatParseHandling = FloatParseHandling.Decimal,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = DateParseHandling.None,
                NullValueHandling = NullValueHandling.Ignore
            });

            SaveInvoice(output2, item.InternalId);

            JObject unsignedDocument = ParseDocumentJsonPreservingPrimitiveValues(output2);

            // ===================== Sign =====================

            SignedInvoiceDocument signedInvoice;
            try
            {
                signedInvoice = invoiceSigningService.SignDocument(unsignedDocument, item.BillNo, item.InternalId);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "ETA signing failed. BillNo: {BillNo}, InternalId: {InternalId}",
                    item.BillNo,
                    item.InternalId);

                // ← حفظ الفاتورة الفاشلة في مجلد Failed
                SaveInvoice(output2, item.InternalId + "_failed", "Failed");

                billUploadStatusService.MarkRejected(
                    Guid.Parse(item.InternalId),
                    "SigningFailed",
                    "Signing failed: " + ex.Message,
                    null);

                return new BillResponse()
                {
                    BillNo = item.BillNo,
                    Msg = "Signing failed: " + ex.Message,
                    BillGuid = item.InternalId
                };
            }

            string output3 = signedInvoice.SignedDocument.ToString(Formatting.None);

            // ← حفظ الفاتورة الموقعة في مجلد Sent
            SaveInvoice(output3, item.InternalId + "_signed", "Sent");

            logger.LogInformation(
                "ETA final signed document prepared. BillNo: {BillNo}, InternalId: {InternalId}, CanonicalSha256: {CanonicalSha256}, SignatureBase64Length: {SignatureBase64Length}, SignedJson: {SignedJson}",
                item.BillNo,
                item.InternalId,
                signedInvoice.CanonicalContentSha256,
                signedInvoice.Signature?.Length ?? 0,
                output3);

            JObject JSONInvoice = new JObject
            {
                ["documents"] = new JArray(signedInvoice.SignedDocument)
            };

            // ===================== Submit =====================

            try
            {
                EtaSubmissionResult result = etaSubmissionService
                    .SubmitDocumentsAsync(JSONInvoice, item.BillNo, item.InternalId)
                    .GetAwaiter()
                    .GetResult();

                if (result == null)
                {
                    // ← فشل الإرسال - احفظ في Failed
                    SaveInvoice(output3, item.InternalId + "_submit_failed", "Failed");

                    billUploadStatusService.MarkRejected(
                        Guid.Parse(item.InternalId),
                        "NoResponse",
                        "Upload process failed, Please try again later",
                        null);

                    return new BillResponse()
                    {
                        BillNo = item.BillNo,
                        Msg = "Upload process failed, Please try again later",
                        BillGuid = item.InternalId
                    };
                }

                if (result.IsDuplicatePayload)
                {
                    string duplicateMessage = "ETA rejected this retry because the request payload is identical to one sent in the last 10 minutes. Please wait and retry, or check ETA portal for the earlier submission.";

                    billUploadStatusService.MarkDuplicate(
                        Guid.Parse(item.InternalId),
                        duplicateMessage,
                        result.RawResponse);

                    return new BillResponse()
                    {
                        BillNo = item.BillNo,
                        Msg = duplicateMessage,
                        BillGuid = item.InternalId
                    };
                }

                DocumetSubmitResponse etaResponse = result.Response;

                if (etaResponse == null)
                {
                    string failureMessage = EtaResponseFormatter.BuildRawFailureMessage(result);

                    // ← فشل الإرسال - احفظ في Failed
                    SaveInvoice(output3, item.InternalId + "_submit_failed", "Failed");

                    billUploadStatusService.MarkRejected(
                        Guid.Parse(item.InternalId),
                        "Failed",
                        failureMessage,
                        result.RawResponse);

                    return new BillResponse()
                    {
                        BillNo = item.BillNo,
                        Msg = failureMessage,
                        BillGuid = item.InternalId
                    };
                }

                AcceptedDocument acceptedDocument = null;
                if (etaResponse.acceptedDocuments != null && etaResponse.acceptedDocuments.Count > 0)
                {
                    acceptedDocument = etaResponse.acceptedDocuments
                        .FirstOrDefault(x => string.Equals(x.internalId, obj.internalID, StringComparison.OrdinalIgnoreCase))
                        ?? etaResponse.acceptedDocuments.First();
                }

                if (!string.IsNullOrWhiteSpace(etaResponse.submissionUUID) || acceptedDocument != null)
                {
                    billUploadStatusService.MarkAccepted(
                        Guid.Parse(item.InternalId),
                        etaResponse.submissionUUID,
                        acceptedDocument,
                        result.RawResponse);

                    return new BillResponse()
                    {
                        BillNo = item.BillNo,
                        Msg = "Invoice Uploaded successfully",
                        BillGuid = item.InternalId
                    };
                }

                if (etaResponse.rejectedDocuments != null && etaResponse.rejectedDocuments.Count > 0)
                {
                    string rejectionMessage = EtaResponseFormatter.BuildRejectedDocumentsMessage(etaResponse.rejectedDocuments);

                    // ← مرفوضة من ETA - احفظ في Failed
                    SaveInvoice(output3, item.InternalId + "_rejected", "Failed");

                    billUploadStatusService.MarkRejected(
                        Guid.Parse(item.InternalId),
                        "Rejected",
                        rejectionMessage,
                        result.RawResponse);

                    return new BillResponse()
                    {
                        BillNo = item.BillNo,
                        Msg = rejectionMessage,
                        BillGuid = item.InternalId
                    };
                }

                // ← فشل عام - احفظ في Failed
                SaveInvoice(output3, item.InternalId + "_failed", "Failed");

                billUploadStatusService.MarkRejected(
                    Guid.Parse(item.InternalId),
                    "Failed",
                    "Upload process failed, Please try again later",
                    result.RawResponse);

                return new BillResponse()
                {
                    BillNo = item.BillNo,
                    Msg = "Upload process failed, Please try again later",
                    BillGuid = item.InternalId
                };
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "ETA submission failed. BillNo: {BillNo}, InternalId: {InternalId}",
                    item.BillNo,
                    item.InternalId);

                // ← exception في الإرسال - احفظ في Failed
                SaveInvoice(output3, item.InternalId + "_exception", "Failed");

                billUploadStatusService.MarkRejected(
                    Guid.Parse(item.InternalId),
                    "Exception",
                    ex.Message,
                    null);

                return new BillResponse()
                {
                    BillNo = item.BillNo,
                    Msg = ex.Message,
                    BillGuid = item.InternalId
                };
            }
        }

        [HttpGet]
        [Route("GetUploadedInvoices2")]
        public IActionResult GetUploadedInvoices2(
            [FromQuery(Name = "BillType")] Guid? billType = null,
            [FromQuery(Name = "DateFrom")] DateTime? dateFrom = null,
            [FromQuery(Name = "DateTo")] DateTime? dateTo = null)
        {
            try
            {
                var query = this.eInvoiceMasterRepos.GetAll()?.AsQueryable()
                            ?? Enumerable.Empty<VwEInvoiceMaster>().AsQueryable();

                query = query.Where(x => x.IsUploaded == true);

                if (billType.HasValue)
                    query = query.Where(x => x.TypeGuid == billType.Value);

                if (dateFrom.HasValue && dateTo.HasValue)
                    query = query.Where(x => x.Date.HasValue
                        && x.Date.Value >= dateFrom.Value
                        && x.Date.Value <= dateTo.Value);

                return Ok(query.ToList());
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to get uploaded invoices");
                return BadRequest(new { Message = "حدث خطأ", Error = ex.Message });
            }
        }

        /// <summary>
        /// يحفظ محتوى الفاتورة JSON في المسار المحدد.
        /// subFolder: "" = root folder | "Sent" = فواتير اترسلت | "Failed" = فواتير فاشلة
        /// </summary>
        private void SaveInvoice(string strinvoice, string filename, string subFolder = "")
        {
            Appsettings settings = Configuration.GetRequiredSection("Settings").Get<Appsettings>();
            string basePath = @"C:\Invoices";

            if (settings.InvoiceFolderPath != null && !string.IsNullOrEmpty(settings.InvoiceFolderPath))
                basePath = settings.InvoiceFolderPath;

            string path = string.IsNullOrEmpty(subFolder)
                ? basePath
                : Path.Combine(basePath, subFolder);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string filepath = Path.Combine(path, filename + ".json");
            System.IO.File.WriteAllBytes(filepath, System.Text.Encoding.UTF8.GetBytes(strinvoice));

            logger.LogInformation("Invoice payload saved. FilePath: {InvoicePayloadPath}", filepath);
        }

        private static JObject ParseDocumentJsonPreservingPrimitiveValues(string json)
        {
            using (StringReader stringReader = new StringReader(json))
            using (JsonTextReader jsonReader = new JsonTextReader(stringReader))
            {
                jsonReader.DateParseHandling = DateParseHandling.None;
                jsonReader.FloatParseHandling = FloatParseHandling.Decimal;
                return JObject.Load(jsonReader);
            }
        }

        [HttpPost]
        [Route("UploadInvoices")]
        public List<BillResponse> UploadInvoices(List<VwEInvoiceMasterdto> itemList)
        {
            List<BillResponse> uploadeditems = new List<BillResponse>();

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