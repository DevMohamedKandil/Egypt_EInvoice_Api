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
    public class BillController : ControllerBase 
    {
        private readonly IBaseRepos<Bill> BillRepos;


        public BillController(IBaseRepos<Bill> BillRepos)
        {
            this.BillRepos = BillRepos;
        }

        [HttpPost]
        [Route("Update")]
        public bool Update(Guid billguid)
        {
            Bill billObject = BillRepos.FindByGuid(billguid);

            if (billObject != null)
            {
                billObject.IsUploaded = true;
                BillRepos.Update(billObject);
                return true;
            }
            return false;
        }



       
    }
}
