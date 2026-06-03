using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Egypt_EInvoice_Api.Models;

namespace Egypt_EInvoice_Api.Services
{
    // Service to run safe raw SQL queries for views and map to DTOs
    public class SafeQueryExecutor
    {
        private readonly EInvoiceDBContext _context;
        private readonly ILogger<SafeQueryExecutor> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SafeQueryExecutor(EInvoiceDBContext context, ILogger<SafeQueryExecutor> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        // Generic query that maps to DTO registered as keyless in DbContext
        public async Task<List<TDto>> QueryAsync<TDto>(string sql) where TDto : class
        {
            var correlationId = _httpContextAccessor?.HttpContext?.TraceIdentifier ?? "-";
            _logger.LogInformation("[{CorrelationId}] Executing view SQL: {Sql}", correlationId, sql);

            var sw = Stopwatch.StartNew();
            try
            {
                var list = await _context.Set<TDto>().FromSqlRaw(sql).AsNoTracking().ToListAsync();
                sw.Stop();
                _logger.LogInformation("[{CorrelationId}] View query completed in {ElapsedMs}ms. Rows: {Count}", correlationId, sw.ElapsedMilliseconds, list?.Count ?? 0);
                return list;
            }
            catch (System.Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "[{CorrelationId}] View query failed after {ElapsedMs}ms. SQL: {Sql}", correlationId, sw.ElapsedMilliseconds, sql);
                throw;
            }
        }
    }
}
