using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.Models
{
    public class EInvoice_CompanyInfo
    {
        public Guid Guid { get; set; }
        public int Id { get; set; }
        public string ActivityCode { get; set; }
        public string IssuerType { get; set; }
        public string TaxRegisterationNo { get; set; }
        public string CountryCode { get; set; }
        public string Governate { get; set; }
        public string RegionCity { get; set; }
        public string Street { get; set; }
        public string BuildingNumber { get; set; }
        public string PostalCode { get; set; }
        public string FloorNo { get; set; }
        public string LandMark { get; set; }
        public string AdditionalInformation { get; set; }
        public string Room { get; set; }

    }
}
