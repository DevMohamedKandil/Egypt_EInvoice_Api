using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Egypt_EInvoice_Api.Models;

namespace Egypt_EInvoice_Api.Repos
{
    public class BillItemsRepos : IBaseRepos<VWInvoiceLine>
    {
        private readonly EInvoiceDBContext context;
        public BillItemsRepos(EInvoiceDBContext context)
        {
            this.context = context;
        }

        public BillItemsRepos()
        {
            this.context = new EInvoiceDBContext();
        }



        public VWInvoiceLine Add(VWInvoiceLine item)
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

        public List<VWInvoiceLine> SearchByGuid(Guid guid)
        {
            
            return this.context.vwEInvoiceLines.Where(x => x.BillGuid == guid).ToList();
        }

        public VWInvoiceLine FindById(int id)
        {
            throw new NotImplementedException();
        }

        public List<VWInvoiceLine> GetAll()
        {
            return this.context.vwEInvoiceLines.ToList();
        }

        public VWInvoiceLine Update(VWInvoiceLine item)
        {
            throw new NotImplementedException();
        }

        public bool UpdateList(List<VWInvoiceLine> items)
        {
            throw new NotImplementedException();
        }

        VWInvoiceLine IBaseRepos<VWInvoiceLine>.FindByGuid(Guid guid)
        {
            throw new NotImplementedException();
        }
    }
}
