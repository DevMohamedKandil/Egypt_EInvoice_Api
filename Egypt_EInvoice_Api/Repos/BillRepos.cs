using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Egypt_EInvoice_Api.Models;


namespace Egypt_EInvoice_Api.Repos
{
    public class BillRepos : IBaseRepos<Bill>
    {
        private readonly EInvoiceDBContext context;
        public BillRepos(EInvoiceDBContext context)
        {
            this.context = context;
        }
        public Bill Add(Bill item)
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

        public Bill FindByGuid(Guid guid)
        {

            return this.context.Bill.FirstOrDefault(x => x.Guid == guid);
        }

        public Bill FindById(int id)
        {
            throw new NotImplementedException();
        }

        public List<Bill> GetAll()
        {
            return this.context.Bill.ToList();
        }

        public List<Bill> SearchByGuid(Guid guid)
        {
            throw new NotImplementedException();
        }

        public Bill Update(Bill item)
        {
            return UpdateBill(item);
        }

        public bool UpdateList(List<Bill> items)
        {
            throw new NotImplementedException();
        }

        public Bill UpdateBill(Bill item)
        {
            try
            {

                Bill Bill = FindByGuid(item.Guid);
                Bill.IsUploaded = true;

                this.context.SaveChanges();
                return item;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                throw;
            }
            
        }


    }
}
