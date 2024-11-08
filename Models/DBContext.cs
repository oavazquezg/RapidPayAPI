using Microsoft.EntityFrameworkCore;
using RapidPayAPI.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<CreditCard> CreditCards { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<CreditCard>()
            .HasKey(c => c.CardNumber);
        
        modelBuilder.Entity<CreditCard>()
            .HasOne(c => c.Customer)
            .WithMany(c => c.CreditCards)
            .HasForeignKey(c => c.CustomerId);
    }

}