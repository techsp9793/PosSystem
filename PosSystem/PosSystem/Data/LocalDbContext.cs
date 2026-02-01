using Microsoft.EntityFrameworkCore;
using PosSystem.Data.Entities;

namespace PosSystem.Data
{
    public class LocalDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=pos_local_core.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // [CRITICAL] SQLite does not support 'decimal'. We must convert to 'double'.
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.TotalAmount).HasConversion<double>();
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(e => e.UnitPrice).HasConversion<double>();
                // [CRITICAL] Ignore TotalPrice because it is a calculated property (no setter)
                entity.Ignore(e => e.TotalPrice);
            });
        }
    }
}