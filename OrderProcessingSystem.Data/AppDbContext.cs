using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<TransException> TransExceptions => Set<TransException>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure TransException entity
        modelBuilder.Entity<TransException>(entity =>
        {
            entity.HasKey(e => e.TransExceptionId);
            
            entity.Property(e => e.TransactionType)
                  .IsRequired()
                  .HasMaxLength(50);
            
            entity.Property(e => e.InputMessage)
                  .IsRequired()
                  .HasColumnType("TEXT"); // For large JSON messages
            
            entity.Property(e => e.Reason)
                  .IsRequired()
                  .HasMaxLength(1000);
            
            entity.Property(e => e.RunTime)
                  .IsRequired()
                  .HasDefaultValueSql("datetime('now')"); // SQLite default constraint
        });
    }
}
