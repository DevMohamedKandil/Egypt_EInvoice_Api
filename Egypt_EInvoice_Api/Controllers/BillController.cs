using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Egypt_EInvoice_Api.Models;
using Egypt_EInvoice_Api.Repos;
using Egypt_EInvoice_Api.BLL;
using Egypt_EInvoice_Api.Services;

namespace Egypt_EInvoice_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase 
    {
        private readonly IBaseRepos<Bill> BillRepos;
        private readonly IBillUploadStatusService billUploadStatusService;


        public BillController(IBaseRepos<Bill> BillRepos, IBillUploadStatusService billUploadStatusService)
        {
            this.BillRepos = BillRepos;
            this.billUploadStatusService = billUploadStatusService;
        }

        [HttpPost]
        [Route("Update")]
        public bool Update(Guid billguid)
        {
            return billUploadStatusService.MarkAccepted(
                billguid,
                null,
                null,
                "Marked uploaded through legacy Bill/Update endpoint") != null;
        }



       
    }
}
