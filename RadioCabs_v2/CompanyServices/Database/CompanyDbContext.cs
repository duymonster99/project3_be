using CompanyServices.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyServices.Database;

public class CompanyDbContext : DbContext
{
    public CompanyDbContext(DbContextOptions<CompanyDbContext> options) : base(options)
    {
    }
    
    public DbSet<Company> Companies { get; set; }
    public DbSet<CompanyService> CompanyServices { get; set; }
    public DbSet<CompanyLocationService> CompanyLocationServices { get; set; }
    public DbSet<AdvertisementImage> AdvertisementImages { get; set; }
    public DbSet<Payment> Payments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<Models.Company>()
            .HasMany(c => c.CompanyServices)
            .WithOne(cs => cs.Company)
            .HasForeignKey(cs => cs.CompanyId);
        
        modelBuilder.Entity<Models.Company>()
            .HasMany(c => c.CompanyLocationServices)
            .WithOne(cls => cls.Company)
            .HasForeignKey(cls => cls.CompanyId);
        
        modelBuilder.Entity<Company>()
            .HasMany(c => c.Advertisements)
            .WithOne(ai => ai.Company)
            .HasForeignKey(ai => ai.CompanyId);

        modelBuilder.Entity<Company>()
            .HasOne(c => c.Payments)
            .WithOne(c => c.Company)
            .HasForeignKey<Payment>(p => p.CompanyId);
    }
}