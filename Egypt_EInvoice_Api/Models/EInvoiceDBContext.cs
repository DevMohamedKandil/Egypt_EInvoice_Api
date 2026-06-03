using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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

        // ================= TABLES =================
        public virtual DbSet<EInvoice_CompanyInfo> EInvoice_CompanyInfos { get; set; }
        public virtual DbSet<VWItem> vwItems { get; set; }
        public virtual DbSet<VWInvoiceLine> vwEInvoiceLines { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<BillType> BillType { get; set; }
        public virtual DbSet<Bill> Bill { get; set; }

        // ================= VIEWS =================
        public virtual DbSet<VwEInvoiceMaster> VwEInvoiceMasters { get; set; }
        public virtual DbSet<VWEInvoice> VWEInvoices { get; set; }
        public virtual DbSet<VWInvoiceLineDto> VWInvoiceLineDtos { get; set; } // ← أضف


        // ================= CONFIG =================
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

            // =========================================================
            // VWEInvoice — SQL columns are float → C# double
            // =========================================================
            modelBuilder.Entity<VWEInvoice>(entity =>
            {
                entity.HasNoKey();
                entity.Property(e => e.AddTax).HasColumnType("float");
                entity.Property(e => e.NetAmount).HasColumnType("float");
                entity.Property(e => e.TotalAmount).HasColumnType("float");
                entity.Property(e => e.TotalDiscountAmount).HasColumnType("float");
                entity.Property(e => e.TotalSalesAmount).HasColumnType("float");
                entity.Property(e => e.ExtraDiscountAmount).HasColumnType("float");
                entity.Property(e => e.TotalItemsDiscountAmount).HasColumnType("float");
            });

            // =========================================================
            // VwEInvoiceMaster — SQL columns are float → C# double
            // =========================================================
            modelBuilder.Entity<VwEInvoiceMaster>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("vwEInvoiceMasters");
                entity.Property(x => x.TotalSalesAmount).HasColumnType("float");
                entity.Property(x => x.TotalDiscountAmount).HasColumnType("float");
                entity.Property(x => x.NetAmount).HasColumnType("float");
                entity.Property(x => x.TotalAmount).HasColumnType("float");
                entity.Property(x => x.AddTax).HasColumnType("float");
                entity.Property(x => x.ExtraDiscountAmount).HasColumnType("float");
                entity.Property(x => x.TotalItemsDiscountAmount).HasColumnType("float");
            });
            // =========================================================
            // VWInvoiceLineDto — keyless DTO for SafeQueryExecutor
            // =========================================================
            modelBuilder.Entity<VWInvoiceLineDto>(entity =>
            {
                entity.HasNoKey();
                entity.Property(x => x.amountEGP).HasColumnType("decimal(18,5)");
                entity.Property(x => x.amountSold).HasColumnType("decimal(18,5)");
                entity.Property(x => x.salesTotal).HasColumnType("decimal(18,5)");
                entity.Property(x => x.quantity).HasColumnType("decimal(18,5)");
                entity.Property(x => x.currencyExchangeRate).HasColumnType("decimal(18,5)");
                entity.Property(x => x.total).HasColumnType("decimal(18,5)");
                entity.Property(x => x.valueDifference).HasColumnType("decimal(18,5)");
                entity.Property(x => x.totalTaxableFees).HasColumnType("decimal(18,5)");
                entity.Property(x => x.netTotal).HasColumnType("decimal(18,5)");
                entity.Property(x => x.itemsDiscount).HasColumnType("decimal(18,5)");
                entity.Property(x => x.discRate).HasColumnType("decimal(18,5)");
                entity.Property(x => x.discAmount).HasColumnType("decimal(18,5)");
                entity.Property(x => x.AddTax).HasColumnType("decimal(18,5)");
                entity.Property(x => x.TaxPercent).HasColumnType("decimal(18,5)");
            });
            // =========================================================
            // VWInvoiceLine — SQL columns are float → C# double
            // =========================================================
            modelBuilder.Entity<VWInvoiceLine>(entity =>
            {
                entity.HasNoKey();
                entity.Property(x => x.amountEGP).HasColumnType("float");
                entity.Property(x => x.amountSold).HasColumnType("float");
                entity.Property(x => x.salesTotal).HasColumnType("float");
                entity.Property(x => x.quantity).HasColumnType("float");
                entity.Property(x => x.currencyExchangeRate).HasColumnType("float");
                entity.Property(x => x.total).HasColumnType("float");
                entity.Property(x => x.valueDifference).HasColumnType("float");
                entity.Property(x => x.totalTaxableFees).HasColumnType("float");
                entity.Property(x => x.netTotal).HasColumnType("float");
                entity.Property(x => x.itemsDiscount).HasColumnType("float");
                entity.Property(x => x.discRate).HasColumnType("float");
                entity.Property(x => x.discAmount).HasColumnType("float");
                entity.Property(x => x.AddTax).HasColumnType("float");
                entity.Property(x => x.TaxPercent).HasColumnType("float");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}