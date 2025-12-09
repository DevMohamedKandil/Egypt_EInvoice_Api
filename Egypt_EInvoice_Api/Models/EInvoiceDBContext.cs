using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System.IO;

#nullable disable

namespace Egypt_EInvoice_Api.Models
{
    public partial class EInvoiceDBContext : DbContext
    {
        public EInvoiceDBContext()
        {
        }

        public EInvoiceDBContext(DbContextOptions<EInvoiceDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<EInvoice_CompanyInfo> EInvoice_CompanyInfos { get; set; }
        public virtual DbSet<VWItem> vwItems { get; set; }
        public virtual DbSet<VWInvoiceLine> vwEInvoiceLines { get; set; }
        public virtual DbSet<VWEInvoice> vwEInvoices { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Group> Groups { get; set; }

        public virtual DbSet<BillType> BillType { get; set; }//


        public virtual DbSet<Bill> Bill { get; set; }//

        public virtual DbSet<VwEInvoiceMaster> VwEInvoiceMasters { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                                                .SetBasePath(Directory.GetCurrentDirectory())
                                                .AddJsonFile("appsettings.json")
                                                .Build();
                var connectionString = configuration.GetConnectionString("EInvoiceDb");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Arabic_CI_AI");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
