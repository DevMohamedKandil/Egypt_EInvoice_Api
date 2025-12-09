using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Egypt_EInvoice_Api.Models;

namespace Egypt_EInvoice_Api.Repos
{
    public class VWEInvoiceRepos : IBaseRepos<VWEInvoice>
    {
        private readonly EInvoiceDBContext context;
        public VWEInvoiceRepos(EInvoiceDBContext context)
        {
            this.context = context;
        }
        public VWEInvoice Add(VWEInvoice item)
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

        public VWEInvoice FindByGuid(Guid guid)
        {
            return this.context.vwEInvoices.SingleOrDefault(x => x.InternalId == guid.ToString());
        }

        public VWEInvoice FindById(int id)
        {
            throw new NotImplementedException();
        }

        public List<VWEInvoice> GetAll()
        {
            return this.context.vwEInvoices.ToList();
        }

        public List<VWEInvoice> SearchByGuid(Guid guid)
        {
            throw new NotImplementedException();
        }

        public VWEInvoice Update(VWEInvoice item)
        {
            throw new NotImplementedException();
        }

        public bool UpdateList(List<VWEInvoice> items)
        {
            throw new NotImplementedException();
        }
    }
}
