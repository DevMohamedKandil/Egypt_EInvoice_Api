using Egypt_EInvoice_Api.BLL;
using Egypt_EInvoice_Api.Models;
using Egypt_EInvoice_Api.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IBaseRepos<VWItem> itemRepos;
        private readonly IConfiguration _configuration;  
        private readonly ILogger<ItemsController> _logger;

        public ItemsController(IBaseRepos<VWItem> itemRepos, IConfiguration configuration,
                ILogger<ItemsController> logger)
        {
            this.itemRepos = itemRepos;
            this._configuration = configuration;
            this._logger = logger;

        }

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
            _logger.LogInformation("=== UploadCode START === Item: {Name}, EGSCode: {EGSCode}, GPCCode: {GPCCode}",
                item?.Name, item?.EGSCode, item?.GPCCode);

            try
            {
                if (item == null)
                {
                    _logger.LogWarning("UploadCode FAILED: item is null");
                    return BadRequest(new { Message = "Item data is required" });
                }

                if (string.IsNullOrWhiteSpace(item.EGSCode))
                {
                    _logger.LogWarning("UploadCode FAILED: EGSCode is empty for item {Name}", item.Name);
                    return BadRequest(new { Message = "EGSCode is required" });
                }

                if (string.IsNullOrWhiteSpace(item.GPCCode))
                {
                    _logger.LogWarning("UploadCode FAILED: GPCCode is empty for item {Name}", item.Name);
                    return BadRequest(new { Message = "GPCCode is required" });
                }

                if (string.IsNullOrWhiteSpace(item.Name))
                {
                    _logger.LogWarning("UploadCode FAILED: Name is empty");
                    return BadRequest(new { Message = "Item name is required" });
                }

                var activeTo = GetActiveTo();
                _logger.LogInformation("UploadCode - ActiveTo: {ActiveTo}", activeTo?.ToString() ?? "null (no expiry)");

                var obj = new EInvoiceModel.ESGItem
                {
                    codeType = "EGS",
                    activeFrom = DateTime.Now,
                    activeTo = activeTo,
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

                _logger.LogInformation("UploadCode - ESGItem built: codeType={CodeType}, itemCode={ItemCode}, parentCode={ParentCode}",
                    obj.codeType, obj.itemCode, obj.parentCode);

                _logger.LogInformation("UploadCode - Attempting ETA Login...");
                EInvoiceGovManager EGovmanager = new EInvoiceGovManager();
                var loginResponse = EGovmanager.Login();

                if (loginResponse == null)
                {
                    _logger.LogError("UploadCode FAILED: ETA Login returned null");
                    return Unauthorized(new { Message = "Login to E-Invoice portal failed. Check credentials or certificate." });
                }

                _logger.LogInformation("UploadCode - ETA Login SUCCESS.");

                _logger.LogInformation("UploadCode - Sending EGS code to ETA...");
                var etaResult = await EGovmanager.CreateESGCodeAsync(list);

                _logger.LogInformation("UploadCode - ETA Response: IsSuccess={IsSuccess}, StatusCode={StatusCode}, Response={RawResponse}",
                    etaResult.IsSuccess, etaResult.StatusCode, etaResult.RawResponse);

                if (!etaResult.IsSuccess)
                {
                    _logger.LogWarning("UploadCode REJECTED by ETA: Status={StatusCode}, Response={RawResponse}",
                        etaResult.StatusCode, etaResult.RawResponse);

                    return BadRequest(new
                    {
                        Message = "ETA rejected the code",
                        EtaStatus = etaResult.StatusCode,
                        EtaResponse = etaResult.RawResponse
                    });
                }

                // حفظ في DB بعد نجاح ETA
                try
                {
                    var connectionString = _configuration.GetConnectionString("EInvoiceDb");
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        SqlCommand comm = new SqlCommand(
                            "UPDATE Mats SET EGSCode=@EGSCode, GPCCode=@GPCCode WHERE Guid=@Guid", con);
                        comm.Parameters.AddWithValue("@EGSCode", item.EGSCode);
                        comm.Parameters.AddWithValue("@GPCCode", item.GPCCode);
                        comm.Parameters.AddWithValue("@Guid", item.Code);
                        comm.ExecuteNonQuery();
                    }
                    _logger.LogInformation("UploadCode - Item saved to DB. EGSCode: {EGSCode}, Guid: {Guid}",
                        item.EGSCode, item.Code);
                }
                catch (Exception dbEx)
                {
                    // ETA نجح بس DB فشل — نرجع warning مش error
                    _logger.LogError(dbEx, "UploadCode - ETA succeeded but DB save failed. Item: {Name}", item.Name);
                    return Ok(new
                    {
                        Message = "Code submitted to ETA successfully but failed to save in database",
                        EtaResponse = etaResult.RawResponse,
                        DbError = dbEx.Message
                    });
                }

                _logger.LogInformation("=== UploadCode SUCCESS === Item: {Name}, EGSCode: {EGSCode}", item.Name, item.EGSCode);
                return Ok(new
                {
                    Message = "Code submitted successfully",
                    EtaResponse = etaResult.RawResponse
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "=== UploadCode EXCEPTION === Item: {Name}, Error: {Message}", item?.Name, ex.Message);
                return StatusCode(500, new
                {
                    Message = "Error while uploading EGS code",
                    Error = ex.Message,
                    StackTrace = ex.StackTrace
                });
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

                var activeTo = GetActiveTo();

                List<EInvoiceModel.ESGItem> list = itemlist.Select(item => new EInvoiceModel.ESGItem
                {
                    codeType = "EGS",
                    activeFrom = DateTime.Now,
                    activeTo = activeTo,
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

                // حفظ في DB بعد نجاح ETA
                try
                {
                    var connectionString = _configuration.GetConnectionString("EInvoiceDb");
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        foreach (var item in itemlist)
                        {
                            SqlCommand comm = new SqlCommand(
                                "UPDATE Mats SET EGSCode=@EGSCode, GPCCode=@GPCCode WHERE Guid=@Guid", con);
                            comm.Parameters.AddWithValue("@EGSCode", item.EGSCode);
                            comm.Parameters.AddWithValue("@GPCCode", item.GPCCode);
                            comm.Parameters.AddWithValue("@Guid", item.Code);
                            comm.ExecuteNonQuery();
                            comm.Parameters.Clear();
                        }
                    }
                    _logger.LogInformation("UploadCodes - {Count} items saved to DB after ETA upload.", itemlist.Count);
                }
                catch (Exception dbEx)
                {
                    _logger.LogError(dbEx, "UploadCodes - ETA succeeded but DB save failed.");
                    return Ok(new
                    {
                        Message = "Codes submitted to ETA successfully but failed to save in database",
                        EtaResponse = etaResult.RawResponse,
                        DbError = dbEx.Message
                    });
                }

                return Ok(new { Message = "Codes submitted successfully", EtaResponse = etaResult.RawResponse });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UploadCodes EXCEPTION: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Error while uploading EGS codes", Error = ex.Message });
            }
        }
    }
}