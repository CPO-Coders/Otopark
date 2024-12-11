using Microsoft.EntityFrameworkCore;
using Otopark.Models;

namespace Otopark.DbContext;



public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    
    public DbSet<User> Users { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<VehicleType> VehicleTypes { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Spot> Spots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User - Car bir-çok ilişki
        modelBuilder.Entity<Car>()
            .HasOne(c => c.User)
            .WithMany(u => u.Cars)
            .HasForeignKey(c => c.UserId);

        // Car - VehicleType çok-bir ilişki
        modelBuilder.Entity<Car>()
            .HasOne(c => c.VehicleType)
            .WithMany(v => v.Cars)
            .HasForeignKey(c => c.TypeId);

        // Spot - VehicleType çok-bir
        modelBuilder.Entity<Spot>()
            .HasOne(s => s.VehicleType)
            .WithMany()
            .HasForeignKey(s => s.TypeId);

        // Ticket - Car çok-bir
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Car)
            .WithMany()
            .HasForeignKey(t => t.CarId);

        // Ticket - Spot çok-bir
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Spot)
            .WithMany(s => s.Tickets)
            .HasForeignKey(t => t.SpotId);
    }
}

