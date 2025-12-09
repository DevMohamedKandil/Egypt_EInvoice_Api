using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Egypt_EInvoice_Api.Models;
using Egypt_EInvoice_Api.Repos;
using Egypt_EInvoice_Api.EInvoiceModel;
using Egypt_EInvoice_Api.BLL;

namespace Egypt_EInvoice_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GovController : ControllerBase
    {
        private readonly EInvoiceGovManager eInvoiceGovManager;
        private readonly EInvoiceDBContext context;

        public GovController(EInvoiceDBContext context)
        {
            this.context = context;
            string apiUrl = "";
            string userName = "";
            string password = "";
            

           // eInvoiceGovManager = new EInvoiceGovManager(apiUrl, userName, password, "");
        }

        [HttpPost]
        [Route("UploadSingleInvoice")]
        public DocumetSubmitResponse UploadSingleInvoice(VWEInvoice invoice)
        {
            var documentList = from inv in context.VwEInvoiceMasters.Where(x => x.InternalId == invoice.InternalId).ToList()
                               join lines in context.vwEInvoiceLines on inv.InternalId equals lines.BillGuid.ToString()                               
                               select new Document { 
                                   documentType=inv.DocumentType,
                                  // dateTimeIssued = inv.DateTimeIssued,
                                   delivery = new Delivery(),
                                   documentTypeVersion = inv.DocumentTypeVersion,
                                   extraDiscountAmount =0,
                                   internalID = inv.InternalId,
                                   //invoiceLines = new List<InvoiceLine>()
                               } ;

            


            // return eInvoiceGovManager.SubmitDocument(documentList);
            return null;
        }
    }
}
