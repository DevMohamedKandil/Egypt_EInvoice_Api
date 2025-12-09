using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class DocumentCancelOrReject
    {
        public string status { get; set; } //Desired status for the document. Must be cancelled to cancel previously issued document.
        public string reason { get; set; } //Reason for cancelling the document.
    }
}
