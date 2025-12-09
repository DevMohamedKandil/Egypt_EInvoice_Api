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
    public class EInvoicesLoginController : ControllerBase
    {

        [HttpGet]
        [Route("Login")]
        public string Login()

        {
            EInvoiceGovManager obj = new EInvoiceGovManager();
            var loginResponse = obj.Login();
            if (loginResponse != null)
            {

                var result = obj.GetAllDocumentTypes();
                return result;
            }
            return "";
        }

       
    }
}
