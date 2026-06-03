using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Egypt_EInvoice_Api.Models;
using Egypt_EInvoice_Api.Services;
using Microsoft.Extensions.Logging;

namespace Egypt_EInvoice_Api.Repos
{
    public class BillItemsRepos : IBaseRepos<VWInvoiceLine>
    {
        private readonly EInvoiceDBContext context;
        private readonly SafeQueryExecutor _safeQuery;
        private readonly Microsoft.Extensions.Logging.ILogger<BillItemsRepos> _logger;
        public BillItemsRepos(EInvoiceDBContext context, SafeQueryExecutor safeQuery, Microsoft.Extensions.Logging.ILogger<BillItemsRepos> logger)
        {
            this.context = context;
            this._safeQuery = safeQuery;
            this._logger = logger;
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
            var sql = $@"SELECT Guid, BillGuid, MatGuid, description, itemType, itemCode,
                    CAST(quantity AS decimal(18,5)) AS quantity, currencySold,
                    CAST(amountEGP AS decimal(18,5)) AS amountEGP,
                    CAST(amountSold AS decimal(18,5)) AS amountSold,
                    CAST(currencyExchangeRate AS decimal(18,5)) AS currencyExchangeRate,
                    CAST(salesTotal AS decimal(18,5)) AS salesTotal,
                    CAST(ISNULL(Total, 0) AS decimal(18,5)) AS total,
                    CAST(valueDifference AS decimal(18,5)) AS valueDifference,
                    CAST(ISNULL(totalTaxableFees, 0) AS decimal(18,5)) AS totalTaxableFees,
                    CAST(netTotal AS decimal(18,5)) AS netTotal,
                    CAST(itemsDiscount AS decimal(18,5)) AS itemsDiscount,
                    CAST(discRate AS decimal(18,5)) AS discRate,
                    CAST(discAmount AS decimal(18,5)) AS discAmount,
                    internalCode,
                    CAST(ISNULL(AddTax, 0) AS decimal(18,5)) AS AddTax,
                    CAST(ISNULL(TaxPercent, 0) AS decimal(18,5)) AS TaxPercent
                    FROM vwEInvoiceLines WHERE BillGuid = '{guid}'";

            _logger?.LogInformation("Executing View Query: {ViewName} in {Repository}.{Method}", "vwEInvoiceLines", nameof(BillItemsRepos), nameof(SearchByGuid));
            try
            {
                var dtos = _safeQuery.QueryAsync<VWInvoiceLineDto>(sql).Result;
                _logger?.LogInformation("Query completed: {Repository}.{Method} - Rows: {Count}", nameof(BillItemsRepos), nameof(SearchByGuid), dtos?.Count ?? 0);
                return dtos.Select(dto => new VWInvoiceLine
                {
                    Guid = dto.Guid,
                    BillGuid = dto.BillGuid,
                    MatGuid = dto.MatGuid,
                    description = dto.description,
                    itemType = dto.itemType,
                    itemCode = dto.itemCode,
                    quantity = dto.quantity,
                    currencySold = dto.currencySold,
                    amountEGP = dto.amountEGP,
                    amountSold = dto.amountSold,
                    currencyExchangeRate = dto.currencyExchangeRate,
                    salesTotal = dto.salesTotal,
                    total = dto.total,
                    valueDifference = dto.valueDifference,
                    totalTaxableFees = dto.totalTaxableFees,
                    netTotal = dto.netTotal,
                    itemsDiscount = dto.itemsDiscount,
                    discRate = dto.discRate,
                    discAmount = dto.discAmount,
                    internalCode = dto.internalCode,
                    AddTax = dto.AddTax,
                    TaxPercent = dto.TaxPercent
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "View query failed in {ViewName} inside {Repository}.{Method}", "vwEInvoiceLines", nameof(BillItemsRepos), nameof(SearchByGuid));
                throw;
            }
        }
        public VWInvoiceLine FindById(int id)
        {
            throw new NotImplementedException();
        }

        public List<VWInvoiceLine> GetAll()
        {
            var sql = @"SELECT Guid, BillGuid, MatGuid, description, itemType, itemCode,
                    CAST(quantity AS decimal(18,5)) AS quantity, currencySold,
                    CAST(amountEGP AS decimal(18,5)) AS amountEGP,
                    CAST(amountSold AS decimal(18,5)) AS amountSold,
                    CAST(currencyExchangeRate AS decimal(18,5)) AS currencyExchangeRate,
                    CAST(salesTotal AS decimal(18,5)) AS salesTotal,
                    CAST(ISNULL(Total, 0) AS decimal(18,5)) AS total,
                    CAST(valueDifference AS decimal(18,5)) AS valueDifference,
                    CAST(ISNULL(totalTaxableFees, 0) AS decimal(18,5)) AS totalTaxableFees,
                    CAST(netTotal AS decimal(18,5)) AS netTotal,
                    CAST(itemsDiscount AS decimal(18,5)) AS itemsDiscount,
                    CAST(discRate AS decimal(18,5)) AS discRate,
                    CAST(discAmount AS decimal(18,5)) AS discAmount,
                    internalCode,
                    CAST(ISNULL(AddTax, 0) AS decimal(18,5)) AS AddTax,
                    CAST(ISNULL(TaxPercent, 0) AS decimal(18,5)) AS TaxPercent
                    FROM vwEInvoiceLines";

            _logger?.LogInformation("Starting query: {Repository} - {Method}", nameof(BillItemsRepos), nameof(GetAll));
            try
            {
                var dtos = _safeQuery.QueryAsync<VWInvoiceLineDto>(sql).Result;
                _logger?.LogInformation("Query completed: {Repository} - {Method} - Rows: {Count}", nameof(BillItemsRepos), nameof(GetAll), dtos?.Count ?? 0);
                return dtos.Select(dto => new VWInvoiceLine
                {
                    Guid = dto.Guid,
                    BillGuid = dto.BillGuid,
                    MatGuid = dto.MatGuid,
                    description = dto.description,
                    itemType = dto.itemType,
                    itemCode = dto.itemCode,
                    quantity = dto.quantity,
                    currencySold = dto.currencySold,
                    amountEGP = dto.amountEGP,
                    amountSold = dto.amountSold,
                    currencyExchangeRate = dto.currencyExchangeRate,
                    salesTotal = dto.salesTotal,
                    total = dto.total,
                    valueDifference = dto.valueDifference,
                    totalTaxableFees = dto.totalTaxableFees,
                    netTotal = dto.netTotal,
                    itemsDiscount = dto.itemsDiscount,
                    discRate = dto.discRate,
                    discAmount = dto.discAmount,
                    internalCode = dto.internalCode,
                    AddTax = dto.AddTax,
                    TaxPercent = dto.TaxPercent
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "View query failed in {ViewName} inside {Repository}.{Method}", "vwEInvoiceLine", nameof(BillItemsRepos), nameof(GetAll));
                throw;
            }
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
