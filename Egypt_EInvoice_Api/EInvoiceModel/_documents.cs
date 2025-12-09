using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class _documents
    {
        public _documents()
        {
            documents = new List<Document>();
        }
        public List<Document> documents { get; set; }
    }
}
