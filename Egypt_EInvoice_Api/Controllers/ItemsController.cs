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
        public ItemsController(IBaseRepos<VWItem> itemRepos)
        {
            this.itemRepos = itemRepos;
        }


        [HttpGet]
        [Route("GetAll")]
        public List<VWItem> GetAll()
        {
            return this.itemRepos.GetAll();
        }

        [HttpPut]
        [Route("Update")]
        public VWItem Update(VWItem item)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                                              .SetBasePath(Directory.GetCurrentDirectory())
                                              .AddJsonFile("appsettings.json")
                                              .Build();
            var connectionString = configuration.GetConnectionString("EInvoiceDb");


            using (SqlConnection con = new SqlConnection($"{connectionString}"))
            {
                try
                {

                    con.Open();
                    StringBuilder str = new StringBuilder();
                    str.Append("update Mats");
                    str.AppendFormat(" set GS1Code = '{0}'", item.GS1Code);
                    str.AppendFormat(", EGSCode = '{0}'", item.EGSCode);
                    str.AppendFormat(", GPCCode = '{0}'", item.GPCCode);
                    str.AppendFormat("where Guid = '{0}'", item.Code);
                    SqlCommand comm = new SqlCommand(str.ToString(), con);
                    comm.ExecuteNonQuery();
                    con.Close();

                }
                catch (Exception ex)
                {


                }
                return item;
            }

            //return this.itemRepos.Update(item);
        }

        [HttpGet]
        [Route("GetItemsByGroup")]
        public List<VWItem> GetItemsByGroup(Guid groupGuid)
        {
            return this.itemRepos.GetAll().Where(x => x.GroupGuid == groupGuid).ToList();
        }

        //

        [HttpPost]
        [Route("Add")]
        public VWItem Add(VWItem item)
        {
            return this.itemRepos.Add(item);
        }
        [HttpPost]
        [Route("UploadCode")]
        public VWItem UploadCode(VWItem item)

        {
            EInvoiceModel.ESGItem obj = new EInvoiceModel.ESGItem();

            obj.codeType = "EGS";
            //obj.activeFrom = DateTime.Now.ToString("M/dd/yyyy");
            //obj.activeTo = DateTime.Now.AddYears(1).ToString("M/dd/yyyy");
            obj.activeFrom = DateTime.Now;
           // obj.activeTo = DateTime.Now.AddYears(1);
           // obj.codeName = item.LatinName;
            obj.codeName = item.Name;


            obj.codeNameAr = item.Name;
           // obj.description = item.LatinName;
            obj.description = item.Name;

            obj.descriptionAr = item.Name;
            obj.itemCode = item.EGSCode;
            obj.linkedCode = "";
            obj.parentCode = item.GPCCode;
            obj.requestReason = "New Product";


            //obj.codeType = "EGS";
            //obj.parentCode = "10000051";
            //obj.itemCode = "EG-113317713-5689";
            //obj.codeName = "Test Code 1";
            //obj.codeNameAr = "1 كود تجريبي";
            //obj.activeFrom = DateTime.Now;
            //obj.activeTo = DateTime.Now.AddYears(1);
            //obj.description = "Description of code 1";
            //obj.descriptionAr = " 1 وصف الكود بالعربي";
            //obj.requestReason = "Request reason text";

            List<EInvoiceModel.ESGItem> list = new List<EInvoiceModel.ESGItem>();
            list.Add(obj);
            EInvoiceGovManager EGovmanager = new EInvoiceGovManager();
            var loginResponse = EGovmanager.Login();
            if (loginResponse != null)
            {

                EGovmanager.CreateESGCode(list);
                return item;



            }
            return null;
        }



        [HttpPost]
        [Route("UploadCodes")]
        public List<VWItem> UploadCodes(List<VWItem> itemlist)

        {
            List<EInvoiceModel.ESGItem> list = new List<EInvoiceModel.ESGItem>();

            foreach (var item in itemlist)
            {
                EInvoiceModel.ESGItem obj = new EInvoiceModel.ESGItem();
                obj.codeType = "EGS";
                //obj.activeFrom = DateTime.Now.ToString("M/dd/yyyy");
                //obj.activeTo = DateTime.Now.AddYears(1).ToString("M/dd/yyyy");
                obj.activeFrom = DateTime.Now;
               // obj.activeTo = DateTime.Now.AddYears(1);
               // obj.codeName = item.LatinName;
                obj.codeName = item.Name;

                obj.codeNameAr = item.Name;
               // obj.description = item.LatinName;
                obj.description = item.Name;

                obj.descriptionAr = item.Name;
                obj.itemCode = item.EGSCode;
                obj.linkedCode = "";
                obj.parentCode = item.GPCCode;
                obj.requestReason = "New Product";
                list.Add(obj);


            }




            EInvoiceGovManager EGovmanager = new EInvoiceGovManager();
            var loginResponse = EGovmanager.Login();
            if (loginResponse != null)
            {
                
                    EGovmanager.CreateESGCode(list);


            }
            return itemlist;
        }



    }

}
