using Egypt_EInvoice_Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Egypt_EInvoice_Api.Repos
{
    public class BillTypeRepos : IBaseRepos<BillType>
    {
        private readonly EInvoiceDBContext context;
        private readonly Microsoft.Extensions.Logging.ILogger<BillTypeRepos> _logger;
        public BillTypeRepos(EInvoiceDBContext context, Microsoft.Extensions.Logging.ILogger<BillTypeRepos> logger)
        {
            this.context = context;
            this._logger = logger;
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
            try
            {
                _logger?.LogInformation("BillTypeRepos.GetAll called");

                return this.context.BillType
                    .AsNoTracking()
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Unhandled repository error in BillTypeRepos.GetAll");
                throw;
            }
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
