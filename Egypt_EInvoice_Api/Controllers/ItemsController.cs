using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Egypt_EInvoice_Api.Models;
using Egypt_EInvoice_Api.Repos;
using Egypt_EInvoice_Api.BLL;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text;

namespace Egypt_EInvoice_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IBaseRepos<VWItem> itemRepos;
        private readonly IConfiguration _configuration; // ← أضف

        public ItemsController(IBaseRepos<VWItem> itemRepos, IConfiguration configuration) // ← أضف
        {
            this.itemRepos = itemRepos;
            this._configuration = configuration; // ← أضف
        }

        // ← helper method واحدة بدل التكرار
        private DateTime? GetActiveTo()
        {
            int years = _configuration.GetValue<int>("Settings:ItemCodeActiveToYears", 0);
            return years == 0 ? (DateTime?)null : DateTime.Now.AddYears(years);
        }

        [HttpGet]
        [Route("GetAll")]
        public List<VWItem> GetAll()
        {
            return this.itemRepos.GetAll();
        }

        [HttpPut]
        [HttpPost]
        [Route("Update")]
        public IActionResult Update(VWItem item)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("EInvoiceDb"); // ← استخدم الـ injected config

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    StringBuilder str = new StringBuilder();
                    str.Append("UPDATE Mats ");
                    str.Append("SET GS1Code = @GS1Code, ");
                    str.Append("EGSCode = @EGSCode, ");
                    str.Append("GPCCode = @GPCCode ");
                    str.Append("WHERE Guid = @Guid");

                    SqlCommand comm = new SqlCommand(str.ToString(), con);
                    comm.Parameters.AddWithValue("@GS1Code", item.GS1Code);
                    comm.Parameters.AddWithValue("@EGSCode", item.EGSCode);
                    comm.Parameters.AddWithValue("@GPCCode", item.GPCCode);
                    comm.Parameters.AddWithValue("@Guid", item.Code);
                    comm.ExecuteNonQuery();
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error occurred while updating item", Error = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetItemsByGroup")]
        public List<VWItem> GetItemsByGroup(Guid groupGuid)
        {
            return this.itemRepos.GetAll().Where(x => x.GroupGuid == groupGuid).ToList();
        }

        [HttpPost]
        [Route("Add")]
        public VWItem Add(VWItem item)
        {
            return this.itemRepos.Add(item);
        }

        [HttpPost]
        [Route("UploadCode")]
        public async Task<IActionResult> UploadCode([FromBody] VWItem item)
        {
            try
            {
                var obj = new EInvoiceModel.ESGItem
                {
                    codeType = "EGS",
                    activeFrom = DateTime.Now,
                    activeTo = GetActiveTo(), // ← من appsettings
                    codeName = item.Name,
                    codeNameAr = item.Name,
                    description = item.Name,
                    descriptionAr = item.Name,
                    itemCode = item.EGSCode,
                    linkedCode = "",
                    parentCode = item.GPCCode,
                    requestReason = "New Product"
                };

                var list = new List<EInvoiceModel.ESGItem> { obj };
                EInvoiceGovManager EGovmanager = new EInvoiceGovManager();
                var loginResponse = EGovmanager.Login();

                if (loginResponse == null)
                    return Unauthorized("Login to E-Invoice failed");

                var etaResult = await EGovmanager.CreateESGCodeAsync(list);

                if (!etaResult.IsSuccess)
                    return BadRequest(new { Message = "ETA rejected the code", EtaStatus = etaResult.StatusCode, EtaResponse = etaResult.RawResponse });

                return Ok(new { Message = "Code submitted successfully", EtaResponse = etaResult.RawResponse });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error while uploading EGS code", Error = ex.Message });
            }
        }

        [HttpPost]
        [Route("UploadCodes")]
        public async Task<IActionResult> UploadCodes([FromBody] List<VWItem> itemlist)
        {
            try
            {
                if (itemlist == null || !itemlist.Any())
                    return BadRequest("Item list is empty");

                var activeTo = GetActiveTo(); // ← احسبها مرة واحدة بس

                List<EInvoiceModel.ESGItem> list = itemlist.Select(item => new EInvoiceModel.ESGItem
                {
                    codeType = "EGS",
                    activeFrom = DateTime.Now,
                    activeTo = activeTo, // ← من appsettings
                    codeName = item.Name,
                    codeNameAr = item.Name,
                    description = item.Name,
                    descriptionAr = item.Name,
                    itemCode = item.EGSCode,
                    linkedCode = "",
                    parentCode = item.GPCCode,
                    requestReason = "New Product"
                }).ToList();

                EInvoiceGovManager EGovmanager = new EInvoiceGovManager();
                var loginResponse = EGovmanager.Login();

                if (loginResponse == null)
                    return Unauthorized("E-Invoice login failed");

                var etaResult = await EGovmanager.CreateESGCodeAsync(list);

                if (!etaResult.IsSuccess)
                    return BadRequest(new { Message = "ETA rejected the codes", EtaStatus = etaResult.StatusCode, EtaResponse = etaResult.RawResponse });

                return Ok(new { Message = "Codes submitted successfully", EtaResponse = etaResult.RawResponse });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error while uploading EGS codes", Error = ex.Message });
            }
        }
    }
}