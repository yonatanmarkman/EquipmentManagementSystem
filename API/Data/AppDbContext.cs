using Microsoft.EntityFrameworkCore;
using API.Entities;

namespace API.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Equipment> EquipmentItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Location> Locations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Call parent implementation

            // Equipment configuration
            modelBuilder.Entity<Equipment>(entity =>
            {
                // 1. Define primary key
                entity.HasKey(e => e.Id);
                
                // 2. Make SerialNumber unique (can't use Data Annotations for this)
                entity.HasIndex(e => e.SerialNumber)
                      .IsUnique();

                // 3. Set default value for Status
                entity.Property(e => e.Status)
                      .HasDefaultValue(EquipmentStatus.Active);

                // 4. Define relationship: Equipment -> Category (many-to-one)
                entity.HasOne(e => e.Category)              // Equipment has one Category
                      .WithMany(c => c.EquipmentItems)      // Category has many Equipment
                      .HasForeignKey(e => e.CategoryId)     // Foreign key is CategoryId
                      .OnDelete(DeleteBehavior.Restrict);   // Don't allow deleting Category if Equipment exists

                // 5. Define relationship: Equipment -> Location (many-to-one)
                entity.HasOne(e => e.Location)
                      .WithMany(l => l.EquipmentItems)
                      .HasForeignKey(e => e.LocationId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Category configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
            });

            // Location configuration
            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(l => l.Id);
            });
        }
    }
}


