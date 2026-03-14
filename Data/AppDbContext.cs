using CosmosApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CosmosApi.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
        public DbSet<Customer> Customers => Set<Customer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(e => e.InvoiceId);
                entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.InvoiceDate).IsRequired();
                entity.Property(e => e.InvoiceAmount).IsRequired().HasColumnType("numeric(18,2)");

                entity.HasOne(e => e.Customer)
                        .WithMany(e => e.Invoices)
                        .HasForeignKey(e => e.CustomerId);
            });

            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.HasKey(e => e.InvoiceItemId);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.Rate).IsRequired().HasColumnType("numeric(18,2)");
                entity.Property(e => e.Amount).IsRequired().HasColumnType("numeric(18,2)");

                entity.HasOne(e => e.Invoice)
                        .WithMany(e => e.InvoiceItems)
                        .HasForeignKey(e => e.InvoiceId);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).IsRequired(false).HasMaxLength(20);
                entity.Property(e => e.Email).IsRequired(false).HasMaxLength(100);
                entity.Property(e => e.Description).IsRequired(false).HasMaxLength(500);
            });
        }
    }
}
