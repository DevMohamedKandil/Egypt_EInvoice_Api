using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Egypt_EInvoice_Api.Models;
using Microsoft.EntityFrameworkCore;
using Egypt_EInvoice_Api.Repos;
using System.Text;

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
            services.AddControllers();
            services.AddDbContext<EInvoiceDBContext>(options => {
                string connectionString = Configuration.GetConnectionString("EInvoiceDb");
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IBaseRepos<EInvoice_CompanyInfo>, Company_InfoRepos>();
            services.AddScoped<IBaseRepos<VWItem>, ItemsRepos>();
            services.AddScoped<IAuthRepos<User>, AuthRepos>();
            services.AddScoped<IBaseRepos<Group>, GroupRepos>();
            services.AddScoped<IBaseRepos<VWEInvoice>, VWEInvoiceRepos>();
            services.AddScoped<IBaseRepos<VwEInvoiceMaster>, VWEInvoiceMasterRepos>();

            services.AddScoped<IBaseRepos<BillType>, BillTypeRepos>();//
            services.AddScoped<IBaseRepos<Bill>, BillRepos>();//

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 2147483647;
            });



            services.AddCors(options => {
                string allowedOriginString = Configuration.GetSection("AllowedOrigins").Value;
                string [] allowedOriginArr = allowedOriginString.Split(',');
                options.AddPolicy(name: allowedOrigins, builder => {
                    builder.WithOrigins(allowedOriginArr.ToArray())
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });


            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
