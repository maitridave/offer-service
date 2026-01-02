using AI.OfferService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AI.OfferService.Domain.Data;

public class OfferDbContext : DbContext
{
    public OfferDbContext(DbContextOptions<OfferDbContext> options) : base(options) { }

    public DbSet<Offer> Offers { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Offer>(entity =>
        {
            entity.ToTable("Offer");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.OfferAmount).HasColumnType("decimal(12,2)");
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(e => e.LastModifiedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            
            entity.HasOne(e => e.Vehicle)
                  .WithMany(v => v.Offers)
                  .HasForeignKey(e => e.VehicleId);
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.ToTable("Vehicle");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Make).HasMaxLength(50);
            entity.Property(e => e.Model).HasMaxLength(100);
            entity.Property(e => e.Trim).HasMaxLength(100);
            entity.Property(e => e.VIN).HasMaxLength(17).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.LastModifiedAt).HasDefaultValueSql("GETUTCDATE()");
        });
    }
}