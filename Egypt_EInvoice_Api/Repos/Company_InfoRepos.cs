using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Egypt_EInvoice_Api.Models;

namespace Egypt_EInvoice_Api.Repos
{
    public class Company_InfoRepos : IBaseRepos<EInvoice_CompanyInfo>
    {
        private readonly EInvoiceDBContext context;
        public Company_InfoRepos(EInvoiceDBContext context)
        {
            this.context = context;
        }
        public EInvoice_CompanyInfo Add(EInvoice_CompanyInfo item)
        {
            if (GetAll().Count > 0)
            {
                return Update(item);
            }
            return Add(item);
        }

        public bool DeleteByGuid(Guid guid)
        {
            EInvoice_CompanyInfo eInvoice_CompanyInfo = FindByGuid(guid);
            this.context.Remove(eInvoice_CompanyInfo);
            this.context.SaveChanges();
            return true;
        }

        public bool DeleteById(int id)
        {
            EInvoice_CompanyInfo eInvoice_CompanyInfo = FindById(id);
            this.context.Remove(eInvoice_CompanyInfo);
            this.context.SaveChanges();
            return true;
        }

        public EInvoice_CompanyInfo FindByGuid(Guid guid)
        {
            return this.context.EInvoice_CompanyInfos.Where(x => x.Guid == guid).First();
        }

        public EInvoice_CompanyInfo FindById(int id)
        {
            return this.context.EInvoice_CompanyInfos.Where(x => x.Id == id).First();
        }

        public List<EInvoice_CompanyInfo> GetAll()
        {
            return this.context.EInvoice_CompanyInfos.ToList();
        }

        public List<EInvoice_CompanyInfo> SearchByGuid(Guid guid)
        {
            throw new NotImplementedException();
        }

        public EInvoice_CompanyInfo Update(EInvoice_CompanyInfo item)
        {
            EInvoice_CompanyInfo eInvoice_CompanyInfo = FindByGuid(item.Guid);
            eInvoice_CompanyInfo.ActivityCode = item.ActivityCode;
            eInvoice_CompanyInfo.AdditionalInformation = item.AdditionalInformation;
            eInvoice_CompanyInfo.BuildingNumber = item.BuildingNumber;
            eInvoice_CompanyInfo.CountryCode = item.CountryCode;
            eInvoice_CompanyInfo.FloorNo = item.FloorNo;
            eInvoice_CompanyInfo.Governate = item.Governate;
            eInvoice_CompanyInfo.IssuerType = item.IssuerType;
            eInvoice_CompanyInfo.LandMark = item.LandMark;
            eInvoice_CompanyInfo.PostalCode = item.PostalCode;
            eInvoice_CompanyInfo.RegionCity = item.RegionCity;
            eInvoice_CompanyInfo.Street = item.Street;
            eInvoice_CompanyInfo.TaxRegisterationNo = item.TaxRegisterationNo;
            eInvoice_CompanyInfo.Room = item.Room;
            this.context.SaveChanges();
            return eInvoice_CompanyInfo;
                
        }

        public bool UpdateList(List<EInvoice_CompanyInfo> items)
        {
            throw new NotImplementedException();
        }
    }
}
