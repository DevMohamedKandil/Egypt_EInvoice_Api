using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.EInvoiceModel
{
    public class Signature
    {
        public string signatureType { get; set; } //Type of the signature: Issuer (I), ServiceProvider (S)
        public string value { get; set; } //Signature value that contains CADES-BES structure containing signer certificate, hash value signed and actual signature value.
    }
}
