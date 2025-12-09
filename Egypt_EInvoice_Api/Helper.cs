using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api
{
    public class Helper
    {
        //public static readonly string client_id = "8fdde084-7a54-4916-8efc-32cb63640ede";
        //public static readonly string client_secret = "e259b23f-e20f-46fa-b858-eb8810eed139";

        //public static readonly string loginUrl = "https://id.preprod.eta.gov.eg";
        //public static readonly string apiUrl = "https://api.preprod.invoicing.eta.gov.eg";

        //actual- first client 
        //  public static readonly string client_id = "219384a9-7347-46f5-82b1-762a88b1ea70";
        //  public static readonly string client_secret = "7e386f3f-e471-4d78-bac3-498c12598f57";
        //Client Secret 2
        //6ad04786-f8bc-4f06-a123-b2fa0d17c633

        // actual second client مصنع الجوهرى
     //   public static readonly string client_id = "8887b0f6-7273-473b-bbcf-5866f2201a6d";
     //  public static readonly string client_secret = "53dda596-3e71-4dbe-8969-792bb94848ae";
        //

        //actual third client
        // public static readonly string client_id = "97f36ef9-6782-429c-afc3-45547a41fa69";
        // public static readonly string client_secret = "236b1ec2-8f23-4d65-bf6f-e3c1934616b1";
        //
      //  public static readonly string loginUrl = "https://id.eta.gov.eg";
      //  public static readonly string apiUrl = "https://api.invoicing.eta.gov.eg";
      //  public static readonly string onBehalf = "";
    }
    public class Appsettings
    {
        public string InvoiceFolderPath { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
       
        public string loginUrl { get; set; }
        public string apiUrl { get; set; }
        public string onBehalf { get; set; }
        public int InvoiceTitle { get; set; }




    }
}
