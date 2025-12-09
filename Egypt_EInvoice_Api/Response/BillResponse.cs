using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.Response
{
    public class BillResponse
    {
        public string BillNo { get; set; }
        public string Msg { get; set; }
        public string BillGuid { get; set; }
    }
}
