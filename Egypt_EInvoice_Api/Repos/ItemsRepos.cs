using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Egypt_EInvoice_Api.Models;

namespace Egypt_EInvoice_Api.Repos
{
    public class ItemsRepos : IBaseRepos<VWItem>
    {
        private readonly EInvoiceDBContext context;
        public ItemsRepos(EInvoiceDBContext context)
        {
            this.context = context;
        }
        public VWItem Add(VWItem item)
        {
            return Add(item);
        }

        public bool DeleteByGuid(Guid guid)
        {
            throw new NotImplementedException();
        }

        public bool DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public VWItem FindByGuid(Guid guid)
        {
            
            return this.context.vwItems.Single(x => x.Code == guid.ToString());
        }

        public VWItem FindById(int id)
        {
            throw new NotImplementedException();
        }

        public List<VWItem> GetAll()
        {
            return this.context.vwItems.ToList();
        }

        public List<VWItem> SearchByGuid(Guid guid)
        {
            throw new NotImplementedException();
        }

        public VWItem Update(VWItem item)
        {
            VWItem updatedItem = FindByGuid(Guid.Parse(item.Code));
            updatedItem.GS1Code = item.GS1Code;
            updatedItem.EGSCode = item.EGSCode;
            updatedItem.GPCCode = item.GPCCode;
          
            this.context.SaveChanges();
            //this.context.vwItems.Update(updatedItem);
            return item;
        }

        public bool UpdateList(List<VWItem> items)
        {
            try
            {
                foreach (VWItem item in items)
                {
                    Update(item);
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
