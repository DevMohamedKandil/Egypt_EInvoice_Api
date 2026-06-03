using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Egypt_EInvoice_Api.Models;

using Microsoft.Extensions.Logging;


namespace Egypt_EInvoice_Api.Repos
{
    public class GroupRepos : IBaseRepos<Group>
    {
        private readonly EInvoiceDBContext context;
        private readonly Microsoft.Extensions.Logging.ILogger<GroupRepos> _logger;
        public GroupRepos(EInvoiceDBContext context, Microsoft.Extensions.Logging.ILogger<GroupRepos> logger)
        {
            this.context = context;
            this._logger = logger;
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
            try
            {
                _logger?.LogInformation("GroupRepos.GetAll called");
                return this.context.Groups.ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Unhandled repository error in GroupRepos.GetAll");
                throw;
            }
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
