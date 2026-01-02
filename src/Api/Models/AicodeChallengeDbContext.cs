using System;
using Microsoft.EntityFrameworkCore;

namespace AI.OfferService.Models
{
    public partial class AicodeChallengeDbContext : DbContext
    {
        public AicodeChallengeDbContext()
        {
        }

        public AicodeChallengeDbContext(DbContextOptions<AicodeChallengeDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Offer> Offers { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Offer>(entity =>
            {
                entity.HasKey(e => e.OfferId).HasName("PK__Offer__0765F09E");

                entity.ToTable("Offer");

                entity.Property(e => e.OfferId).ValueGeneratedOnAdd();
                entity.Property(e => e.City).HasMaxLength(50);
                entity.Property(e => e.Country).HasMaxLength(50);
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime2");
                entity.Property(e => e.OfferAmount).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.State).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime2");
                entity.Property(e => e.VIN).HasMaxLength(17);

                entity.HasOne(d => d.Vehicle).WithMany(p => p.Offers)
                    .HasForeignKey(d => d.VehicleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Offer__VehicleId");
            });

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Vehicle__3214EC07");

                entity.ToTable("Vehicle");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime2");
                entity.Property(e => e.LastModifiedAt)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime2");
                entity.Property(e => e.Make).HasMaxLength(50);
                entity.Property(e => e.Model).HasMaxLength(100);
                entity.Property(e => e.Trim).HasMaxLength(100);
                entity.Property(e => e.VIN).HasMaxLength(17);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
