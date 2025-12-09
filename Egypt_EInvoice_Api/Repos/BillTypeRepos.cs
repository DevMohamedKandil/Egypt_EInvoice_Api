using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Egypt_EInvoice_Api.Models;


namespace Egypt_EInvoice_Api.Repos
{
    public class BillTypeRepos : IBaseRepos<BillType>
    {
        private readonly EInvoiceDBContext context;
        public BillTypeRepos(EInvoiceDBContext context)
        {
            this.context = context;
        }
        public BillType Add(BillType item)
        {
            throw new NotImplementedException();
        }

        public bool DeleteByGuid(Guid guid)
        {
            throw new NotImplementedException();
        }

        public bool DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public BillType FindByGuid(Guid guid)
        {
            throw new NotImplementedException();
        }

        public BillType FindById(int id)
        {
            throw new NotImplementedException();
        }

        public List<BillType> GetAll()
        {
            return this.context.BillType.ToList();
        }

        public List<BillType> SearchByGuid(Guid guid)
        {
            throw new NotImplementedException();
        }

        public BillType Update(BillType item)
        {
            throw new NotImplementedException();
        }

        public bool UpdateList(List<BillType> items)
        {
            throw new NotImplementedException();
        }
    }
}
