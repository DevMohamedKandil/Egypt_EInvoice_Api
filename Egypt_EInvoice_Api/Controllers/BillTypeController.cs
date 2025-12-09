using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Egypt_EInvoice_Api.Models;
using Egypt_EInvoice_Api.Repos;

namespace Egypt_EInvoice_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillTypeController : ControllerBase
    {
        private readonly IBaseRepos<BillType> billTypeRepos;
        public BillTypeController(IBaseRepos<BillType> billTypeRepos)
        {
            this.billTypeRepos = billTypeRepos;
        }



        [HttpGet]
        [Route("GetAll")]
        public List<BillType> GetAll()
        {
            return this.billTypeRepos.GetAll();
        }
       
    }
}
