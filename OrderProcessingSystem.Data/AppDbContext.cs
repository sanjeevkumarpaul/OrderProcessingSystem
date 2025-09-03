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
    public DbSet<UserLog> UserLogs => Set<UserLog>();

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

        // Configure UserLog entity
        modelBuilder.Entity<UserLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.EventDate)
                  .IsRequired();
            
            entity.Property(e => e.Event)
                  .IsRequired()
                  .HasMaxLength(200);
            
            entity.Property(e => e.EventFlag)
                  .IsRequired()
                  .HasMaxLength(50);
            
            entity.Property(e => e.UserId)
                  .IsRequired()
                  .HasMaxLength(255); // Changed to accommodate email addresses
            
            entity.Property(e => e.UserName)
                  .IsRequired()
                  .HasMaxLength(100);
        });
    }
}
