using AuthenticationServices.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationServices.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserInfo> UserInfos { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<DriverInfo> DriverInfos { get; set; }
    public DbSet<FeedbackDriver> FeedbackDrivers { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình mối quan hệ giữa User và UserInfo
        modelBuilder.Entity<User>()
            .HasOne(u => u.UserInfo)
            .WithOne(ui => ui.User)
            .HasForeignKey<UserInfo>(ui => ui.UserId);

        // Cấu hình mối quan hệ giữa Driver và DriverInfo
        modelBuilder.Entity<Driver>()
            .HasOne(d => d.DriverInfo)
            .WithOne(di => di.Driver)
            .HasForeignKey<DriverInfo>(di => di.DriverId);

        modelBuilder.Entity<Driver>()
            .HasMany(d => d.FeedbackDrivers)
            .WithOne(f => f.Driver)
            .HasForeignKey(f => f.DriverId);

        modelBuilder.Entity<Driver>()
            .HasOne(d => d.Booking)
            .WithOne(b => b.Driver)
            .HasForeignKey<Booking>(b => b.DriverId);
    }
}