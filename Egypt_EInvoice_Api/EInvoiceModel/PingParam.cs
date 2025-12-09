using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class PingParam
    {
        public string rin { get; set; } //Taxpayer registration number is shared back to ensure Ping API implementation can verify that it is being configured for the correct taxpayer
    }
}
