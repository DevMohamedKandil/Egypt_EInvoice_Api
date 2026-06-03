using Egypt_EInvoice_Api.Models;
using Egypt_EInvoice_Api.Repos;
using Egypt_EInvoice_Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api
{
    public class Startup
    {
        private readonly string allowedOrigins = "AllowedOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });
            services.AddDbContext<EInvoiceDBContext>(options => {
                string connectionString = Configuration.GetConnectionString("EInvoiceDb");
                options.UseSqlServer(connectionString);

                var aspnetEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var enableEfDiagnostics = string.Equals(aspnetEnv, "Development", StringComparison.OrdinalIgnoreCase)
                                          || Configuration.GetValue<bool>("Settings:EnableEFDiagnostics", false);

                if (enableEfDiagnostics)
                {
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                }
            });

            services.AddHttpContextAccessor();

            services.AddScoped<IBaseRepos<EInvoice_CompanyInfo>, Company_InfoRepos>();
            services.AddScoped<IBaseRepos<VWItem>, ItemsRepos>();
            services.AddScoped<IAuthRepos<User>, AuthRepos>();
            services.AddScoped<IBaseRepos<Group>, GroupRepos>();
            services.AddScoped<IBaseRepos<VWEInvoice>, VWEInvoiceRepos>();
            services.AddScoped<Services.SafeQueryExecutor>();
            services.AddScoped<IBaseRepos<VwEInvoiceMaster>, VWEInvoiceMasterRepos>();
            services.AddScoped<IBaseRepos<VWInvoiceLine>, BillItemsRepos>();

            services.AddScoped<IBaseRepos<BillType>, BillTypeRepos>();
            services.AddScoped<IBaseRepos<Bill>, BillRepos>();
            services.AddScoped<Egypt_EInvoice_Api.BLL.EInvoiceGovManager>();
            services.AddScoped<IEtaAuthService, EtaAuthService>();
            services.AddScoped<IEtaSubmissionService, EtaSubmissionService>();
            services.AddScoped<IInvoiceSigningService, InvoiceSigningService>();
            services.AddScoped<IBillUploadStatusService, BillUploadStatusService>();

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 2147483647;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowedOrigins", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Global exception handler and correlation id middleware
            app.Use(async (context, next) =>
            {
                if (!context.Request.Headers.ContainsKey("X-Correlation-Id"))
                {
                    context.Request.Headers["X-Correlation-Id"] = context.TraceIdentifier;
                }

                context.Response.Headers["X-Correlation-Id"] = context.Request.Headers["X-Correlation-Id"].ToString();
                await next();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var loggerFactory = context.RequestServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
                    var logger = loggerFactory?.CreateLogger("GlobalExceptionHandler");
                    var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
                    var correlationId = context.Request.Headers["X-Correlation-Id"].ToString();

                    logger?.LogError(feature?.Error, "Global Exception captured. Path: {Path} CorrelationId: {CorrelationId}", context.Request.Path, correlationId);

                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new
                    {
                        error = "Internal Server Error",
                        correlationId = correlationId
                    }));
                });
            });

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(allowedOrigins);
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}