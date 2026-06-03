using Egypt_EInvoice_Api.Models;
using Egypt_EInvoice_Api.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Egypt_EInvoice_Api.Repos
{
    public class VWEInvoiceMasterRepos : IBaseRepos<VwEInvoiceMaster>
    {
        private readonly EInvoiceDBContext context;
        private readonly SafeQueryExecutor _safeQuery;
        private readonly ILogger<VWEInvoiceMasterRepos> _logger;

        public VWEInvoiceMasterRepos(
            EInvoiceDBContext context,
            SafeQueryExecutor safeQuery,
            ILogger<VWEInvoiceMasterRepos> logger)
        {
            this.context = context;
            this._safeQuery = safeQuery;
            this._logger = logger;
        }

        // =====================================================================
        // الـ SQL هنا بيعمل CAST صريح لكل الـ float columns لـ double
        // عشان Dapper يقدر يقراها صح بغض النظر عن نوع الـ column في الـ DB
        // =====================================================================

        private const string SelectColumns = @"
            SELECT
                InternalId, [Guid], [Date], BillNo,
                IssuerType, IssuerId, IssuerName,
                IssuerCountryCoder, IssuerGovernate, IssuerRegionCity,
                IssuerStreet, IssuerBuildingNumber, IssuerPostalCode,
                IssuerFloorNo, IssuerRoom, IssuerLandMark, IssuerAdditionalInformation,
                ReceiverType, ReceiverId, ReceiverName,
                ReceiverCountryCode, ReceiverGovernate, ReceiverRegionCity,
                ReceiverStreet, ReceiverBuildingNumber, ReceiverPostalCode,
                ReceiverFloorNo, ReceiverRoom, ReceiverLandMark, ReceiverAdditionalInformation,
                DocumentType, DocumentTypeVersion, ActivityCode,
                PurchaseOrderReference, PurchaseOrderDescription,
                SalesOrderReference, SalesOrderDescription, ProformaInvoiceNumber,
                PaymentBankName, PaymentBankAddress, PaymentBankAccountNo,
                PaymentBankAccountIBAN, PaymentSwiftCode, PaymentTerms,
                DeliveryApproch, DeliveryPackaging, DeliveryDateValidity,
                DeliveryExportPort, DeliveryCountryOfOrigin, DeliveryTerms,
                branchId, EInvoiceGuid,
                IsUploaded, TypeGuid, DateTimeIssued,
                CAST(TotalSalesAmount      AS float) AS TotalSalesAmount,
                CAST(TotalDiscountAmount   AS float) AS TotalDiscountAmount,
                CAST(NetAmount             AS float) AS NetAmount,
                CAST(TotalAmount           AS float) AS TotalAmount,
                CAST(AddTax                AS float) AS AddTax,
                CAST(ExtraDiscountAmount   AS float) AS ExtraDiscountAmount,
                CAST(TotalItemsDiscountAmount AS float) AS TotalItemsDiscountAmount
            FROM vwEInvoiceMasters";

        public List<VwEInvoiceMaster> GetAll()
        {
            _logger?.LogInformation("Starting query: {Repository} - {Method}",
                nameof(VWEInvoiceMasterRepos), nameof(GetAll));
            try
            {
                var sql = SelectColumns;
                var list = _safeQuery.QueryAsync<VwEInvoiceMaster>(sql).Result;
                _logger?.LogInformation("Query completed: {Repository} - {Method} - Rows: {Count}",
                    nameof(VWEInvoiceMasterRepos), nameof(GetAll), list?.Count ?? 0);
                return list ?? new List<VwEInvoiceMaster>();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "View query failed in {Repository}.{Method}",
                    nameof(VWEInvoiceMasterRepos), nameof(GetAll));
                throw;
            }
        }

        public VwEInvoiceMaster FindByGuid(Guid guid)
        {
            _logger?.LogInformation("Executing View Query: {ViewName} in {Repository}.{Method}",
                "vwEInvoiceMasters", nameof(VWEInvoiceMasterRepos), nameof(FindByGuid));
            try
            {
                var sql = SelectColumns + $" WHERE [Guid] = '{guid}'";
                var list = _safeQuery.QueryAsync<VwEInvoiceMaster>(sql).Result;
                _logger?.LogInformation("Query completed: {Repository}.{Method} - Rows: {Count}",
                    nameof(VWEInvoiceMasterRepos), nameof(FindByGuid), list?.Count ?? 0);
                return list?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "View query failed in {Repository}.{Method}",
                    nameof(VWEInvoiceMasterRepos), nameof(FindByGuid));
                throw;
            }
        }

        public List<VwEInvoiceMaster> SearchByGuid(Guid guid)
        {
            throw new NotImplementedException();
        }

        public VwEInvoiceMaster Add(VwEInvoiceMaster item) => throw new NotImplementedException();
        public bool DeleteByGuid(Guid guid) => throw new NotImplementedException();
        public bool DeleteById(int id) => throw new NotImplementedException();
        public VwEInvoiceMaster FindById(int id) => throw new NotImplementedException();
        public VwEInvoiceMaster Update(VwEInvoiceMaster item) => throw new NotImplementedException();
        public bool UpdateList(List<VwEInvoiceMaster> items) => throw new NotImplementedException();
    }
}