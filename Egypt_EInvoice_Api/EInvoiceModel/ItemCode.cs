using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class ItemCode
    {
        public string codeValue { get; set; } //code value of the item code
        public string codeType { get; set; } //type of the code, allowed values are EGS, GS1
    }
}
