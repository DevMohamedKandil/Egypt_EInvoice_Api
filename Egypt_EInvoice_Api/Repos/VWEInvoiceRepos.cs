using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Egypt_EInvoice_Api.Models;
using Egypt_EInvoice_Api.Services;
using Microsoft.Extensions.Logging;

namespace Egypt_EInvoice_Api.Repos
{
    public class VWEInvoiceRepos : IBaseRepos<VWEInvoice>
    {
        private readonly EInvoiceDBContext context;
        private readonly SafeQueryExecutor _safeQuery;
        private readonly Microsoft.Extensions.Logging.ILogger<VWEInvoiceRepos> _logger;
        public VWEInvoiceRepos(EInvoiceDBContext context, SafeQueryExecutor safeQuery, Microsoft.Extensions.Logging.ILogger<VWEInvoiceRepos> logger)
        {
            this.context = context;
            this._safeQuery = safeQuery;
            this._logger = logger;
        }
        public VWEInvoice Add(VWEInvoice item) // Method to add a VWEInvoice
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
            var sql = $@"
        SELECT InternalId, DateTimeIssued, IssuerName, IssuerId,
               CAST(TotalSalesAmount AS decimal(18,5)) AS TotalSalesAmount,
               CAST(TotalDiscountAmount AS decimal(18,5)) AS TotalDiscountAmount,
               CAST(NetAmount AS decimal(18,5)) AS NetAmount,
               CAST(TotalAmount AS decimal(18,5)) AS TotalAmount
        FROM VWEInvoice
        WHERE InternalId = '{guid}'";

            try
            {
                var dto = _safeQuery.QueryAsync<VWEInvoiceDto>(sql).Result.SingleOrDefault();

                if (dto == null) return null;

                return new VWEInvoice
                {
                    InternalId = dto.InternalId,
                    DateTimeIssued = dto.DateTimeIssued,
                    IssuerName = dto.IssuerName,
                    IssuerId = dto.IssuerId,
                    TotalSalesAmount = dto.TotalSalesAmount,
                    TotalDiscountAmount = dto.TotalDiscountAmount,
                    NetAmount = dto.NetAmount,
                    TotalAmount = dto.TotalAmount
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "FindByGuid failed");
                throw;
            }
        }
        public VWEInvoice FindById(int id)
        {
            throw new NotImplementedException();
        }

        public List<VWEInvoice> GetAll()
        {
            var sql = @"SELECT InternalId, DateTimeIssued, IssuerName, IssuerId,
CAST(TotalSalesAmount AS decimal(18,5)) AS TotalSalesAmount,
CAST(TotalDiscountAmount AS decimal(18,5)) AS TotalDiscountAmount,
CAST(NetAmount AS decimal(18,5)) AS NetAmount,
CAST(TotalAmount AS decimal(18,5)) AS TotalAmount
FROM VWEInvoice";

            _logger?.LogInformation("Starting query: {Repository} - {Method}", nameof(VWEInvoiceRepos), nameof(GetAll));
            try
            {
                var dtos = _safeQuery.QueryAsync<VWEInvoiceDto>(sql).Result;
                _logger?.LogInformation("Query completed: {Repository} - {Method} - Rows: {Count}", nameof(VWEInvoiceRepos), nameof(GetAll), dtos?.Count ?? 0);
                return dtos.Select(dto => new VWEInvoice
                {
                    InternalId = dto.InternalId,
                    DateTimeIssued = dto.DateTimeIssued,
                    IssuerName = dto.IssuerName,
                    IssuerId = dto.IssuerId,
                    TotalSalesAmount = dto.TotalSalesAmount,
                    TotalDiscountAmount = dto.TotalDiscountAmount,
                    NetAmount = dto.NetAmount,
                    TotalAmount = dto.TotalAmount
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "View query failed in {ViewName} inside {Repository}.{Method}", "VWEInvoice", nameof(VWEInvoiceRepos), nameof(GetAll));
                throw;
            }
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
