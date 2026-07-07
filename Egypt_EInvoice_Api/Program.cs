using Egypt_EInvoice_Api.BLL;
using Egypt_EInvoice_Api.Models;
using Egypt_EInvoice_Api.Repos;
using Egypt_EInvoice_Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Text;
using Egypt_EInvoice_Api.Services;

string logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
Directory.CreateDirectory(logDirectory);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine(logDirectory, "app-.log"),
        rollingInterval: RollingInterval.Day,
        shared: true,
        retainedFileCountLimit: 31)
    .CreateLogger();

try
{
    Log.Information("Starting Egypt EInvoice API");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // ======= Services =======
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    builder.Services.AddControllers()
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        });

    builder.Services.AddDbContext<EInvoiceDBContext>(options =>
    {
        string connectionString = builder.Configuration.GetConnectionString("EInvoiceDb");
        options.UseSqlServer(connectionString);

        var aspnetEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var enableEfDiagnostics = string.Equals(aspnetEnv, "Development", StringComparison.OrdinalIgnoreCase)
                                  || builder.Configuration.GetValue<bool>("Settings:EnableEFDiagnostics", false);

        if (enableEfDiagnostics)
        {
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
        }
    });

    builder.Services.AddHttpContextAccessor();

    builder.Services.AddScoped<IBaseRepos<EInvoice_CompanyInfo>, Company_InfoRepos>();
    builder.Services.AddScoped<IBaseRepos<VWItem>, ItemsRepos>();
    builder.Services.AddScoped<IAuthRepos<User>, AuthRepos>();
    builder.Services.AddScoped<IBaseRepos<Group>, GroupRepos>();
    builder.Services.AddScoped<IBaseRepos<VWEInvoice>, VWEInvoiceRepos>();
    builder.Services.AddScoped<SafeQueryExecutor>();
    builder.Services.AddScoped<IBaseRepos<VwEInvoiceMaster>, VWEInvoiceMasterRepos>();
    builder.Services.AddScoped<IBaseRepos<VWInvoiceLine>, BillItemsRepos>();
    builder.Services.AddScoped<IBaseRepos<BillType>, BillTypeRepos>();
    builder.Services.AddScoped<IBaseRepos<Bill>, BillRepos>();
    builder.Services.AddScoped<EInvoiceGovManager>();
    builder.Services.AddScoped<IEtaAuthService, EtaAuthService>();
    builder.Services.AddScoped<IEtaSubmissionService, EtaSubmissionService>();
    builder.Services.AddScoped<IInvoiceSigningService, InvoiceSigningService>();
    builder.Services.AddScoped<IBillUploadStatusService, BillUploadStatusService>();

    builder.Services.Configure<IISServerOptions>(options =>
    {
        options.MaxRequestBodySize = 2147483647;
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowedOrigins", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    // ======= Swagger =======
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // ======= Build App =======
    var app = builder.Build();

    // Correlation ID middleware
    app.Use(async (context, next) =>
    {
        if (!context.Request.Headers.ContainsKey("X-Correlation-Id"))
        {
            context.Request.Headers["X-Correlation-Id"] = context.TraceIdentifier;
        }
        context.Response.Headers["X-Correlation-Id"] = context.Request.Headers["X-Correlation-Id"].ToString();
        await next();
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Egypt EInvoice API v1"));
    }

    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var loggerFactory = context.RequestServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
            var logger = loggerFactory?.CreateLogger("GlobalExceptionHandler");
            var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
            var correlationId = context.Request.Headers["X-Correlation-Id"].ToString();

            logger?.LogError(feature?.Error, "Global Exception captured. Path: {Path} CorrelationId: {CorrelationId}",
                context.Request.Path, correlationId);

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                error = "Internal Server Error",
                correlationId = correlationId
            }));
        });
    });

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseCors("AllowedOrigins");
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Egypt EInvoice API terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}