using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Egypt_EInvoice_Api.Models;
using Egypt_EInvoice_Api.Repos;
using Egypt_EInvoice_Api.BLL;

namespace Egypt_EInvoice_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EGItemsController : ControllerBase
    {
        private readonly IBaseRepos<EInvoice_CompanyInfo> companyInfoRepos;
        public EGItemsController(IBaseRepos<EInvoice_CompanyInfo> companyInfoRepos)
        {
            this.companyInfoRepos = companyInfoRepos;
        }

      

        //[HttpPost]
        //[Route("UploadCode")]
        //public EInvoiceModel.ESGItem UploadCode(EInvoiceModel.ESGItem newItem)

        //{
        //    List<EInvoiceModel.ESGItem> list = new List<EInvoiceModel.ESGItem>();
        //    list.Add(newItem);
        //    EInvoiceGovManager obj = new EInvoiceGovManager();
        //    var loginResponse = obj.Login();
        //    if (loginResponse != null)
        //    {

        //        obj.CreateESGCode(list);


        //    }
        //    return newItem;
        //}

        [HttpPost]
        [Route("UploadCode")]
        public EInvoice_CompanyInfo UploadCode(EInvoice_CompanyInfo newItem)

        {
            //List<EInvoiceModel.ESGItem> list = new List<EInvoiceModel.ESGItem>();
            //list.Add(newItem);
            //EInvoiceGovManager obj = new EInvoiceGovManager();
            //var loginResponse = obj.Login();
            //if (loginResponse != null)
            //{

            //    obj.CreateESGCode(list);


            //}
            return newItem;
        }
    }
}
