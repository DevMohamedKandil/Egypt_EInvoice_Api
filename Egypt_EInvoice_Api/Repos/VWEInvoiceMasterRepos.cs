using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Egypt_EInvoice_Api.Models;

namespace Egypt_EInvoice_Api.Repos
{
    public class VWEInvoiceMasterRepos : IBaseRepos<VwEInvoiceMaster>
    {
        private readonly EInvoiceDBContext context;
        public VWEInvoiceMasterRepos(EInvoiceDBContext context)
        {
            this.context = context;
        }
        public VwEInvoiceMaster Add(VwEInvoiceMaster item)
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

        public VwEInvoiceMaster FindByGuid(Guid guid)
        {
            return this.context.VwEInvoiceMasters.SingleOrDefault(x => x.Guid == guid);
        }

        public VwEInvoiceMaster FindById(int id)
        {
            throw new NotImplementedException();
        }

        public List<VwEInvoiceMaster> GetAll()
        {
            return this.context.VwEInvoiceMasters.ToList();
        }

        public List<VwEInvoiceMaster> SearchByGuid(Guid guid)
        {
            throw new NotImplementedException();
        }

        public VwEInvoiceMaster Update(VwEInvoiceMaster item)
        {
            throw new NotImplementedException();
        }

        public bool UpdateList(List<VwEInvoiceMaster> items)
        {
            throw new NotImplementedException();
        }
    }
}
