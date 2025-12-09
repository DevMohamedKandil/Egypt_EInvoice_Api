using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Egypt_EInvoice_Api.Models;


namespace Egypt_EInvoice_Api.Repos
{
    public class GroupRepos : IBaseRepos<Group>
    {
        private readonly EInvoiceDBContext context;
        public GroupRepos(EInvoiceDBContext context)
        {
            this.context = context;
        }
        public Group Add(Group item)
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

        public Group FindByGuid(Guid guid)
        {
            throw new NotImplementedException();
        }

        public Group FindById(int id)
        {
            throw new NotImplementedException();
        }

        public List<Group> GetAll()
        {
            return this.context.Groups.ToList();
        }

        public List<Group> SearchByGuid(Guid guid)
        {
            throw new NotImplementedException();
        }

        public Group Update(Group item)
        {
            throw new NotImplementedException();
        }

        public bool UpdateList(List<Group> items)
        {
            throw new NotImplementedException();
        }
    }
}
