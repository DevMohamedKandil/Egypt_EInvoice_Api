using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class LoginResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public long expires_in { get; set; }
        public string scope { get; set; }

    }
}
