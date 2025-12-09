using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class WorkflowParameter
    {
        public int id { get; set; }
        public string parameter { get; set; }
        public int value { get; set; }
        public DateTime activeFrom { get; set; }
        public DateTime? activeTo { get; set; }
    }
}
