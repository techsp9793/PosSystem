
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PosSystem.Data.Entities;
using PosSystem.Data.Entities.Interfaces;
using PosSystem.Services;
using Microsoft.Extensions.DependencyInjection;

namespace PosSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly IServiceProvider _serviceProvider;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IServiceProvider serviceProvider)
            : base(options)
        {
            _serviceProvider = serviceProvider;
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<SystemPermission> SystemPermissions { get; set; }
        public DbSet<PlanPermission> PlanPermissions { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<BusinessCategory> BusinessCategories { get; set; }
        public DbSet<UnitType> UnitTypes { get; set; }
        public DbSet<TenantTerminology> TenantTerminologies { get; set; }

        public DbSet<Unit> Units { get; set; }
        public DbSet<ItemCategory> Categories { get; set; } // The Product Categories
        public DbSet<Product> Products { get; set; }
        public DbSet<UnitProduct> UnitProducts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // 1. SystemPermission: Ensure 'PermissionCode' is unique
            // (Since PK is now 'Id', we must enforce this constraint manually)
            builder.Entity<SystemPermission>()
                .HasIndex(p => p.PermissionCode)
                .IsUnique();

            // 2. SystemSetting: Composite Uniqueness (TenantId + Key)
            // (Since PK is now 'Id', we ensure a tenant can't have duplicate keys)
            builder.Entity<SystemSetting>()
                .HasIndex(s => new { s.TenantId, s.Key })
                .IsUnique();

            // 3. PlanPermission: Prevent duplicate permissions per plan
            builder.Entity<PlanPermission>()
                .HasIndex(p => new { p.SubscriptionPlanId, p.PermissionCode })
                .IsUnique();
            // --- 2. NEW CONFIGURATION ---

            // Terminology: Unique Key per Tenant (e.g. Tenant A cannot have two "Label_Product" entries)
            builder.Entity<TenantTerminology>()
                .HasIndex(t => new { t.TenantId, t.TermKey }).IsUnique();

            // Unit Inventory Relations
            builder.Entity<UnitProduct>()
                .HasOne(up => up.Unit)
                .WithMany()
                .HasForeignKey(up => up.UnitId)
                .OnDelete(DeleteBehavior.Cascade); // If Unit deleted, stock records gone

            builder.Entity<UnitProduct>()
                .HasOne(up => up.Product)
                .WithMany(p => p.UnitAvailability)
                .HasForeignKey(up => up.ProductId)
                .OnDelete(DeleteBehavior.NoAction); // If Product deleted, handle carefully (usually soft delete)


            // --- 3. SEEDING DEFAULT DATA ---

            // A. Seed Business Categories (Managed by Super Admin)
            builder.Entity<BusinessCategory>().HasData(
                new BusinessCategory
                {
                    Id = "cat-retail-01",
                    Name = "Generic Retail",
                    Description = "Shops, Boutiques, General Stores",
                    DefaultProductTerm = "Product",
                    DefaultCategoryTerm = "Category"
                },
                new BusinessCategory
                {
                    Id = "cat-park-01",
                    Name = "Amusement Park / Attraction",
                    Description = "Theme Parks, Zoos, Water Parks",
                    DefaultProductTerm = "Ticket",
                    DefaultCategoryTerm = "Zone"
                },
                new BusinessCategory
                {
                    Id = "cat-food-01",
                    Name = "Restaurant / Cafe",
                    Description = "Food Service, QSR, Fine Dining",
                    DefaultProductTerm = "Dish",
                    DefaultCategoryTerm = "Menu Section"
                }
            );

            // B. Seed System Default Unit Types (TenantId = null means available to all)
            builder.Entity<UnitType>().HasData(
                new UnitType
                {
                    Id = "type-sales-01",
                    Name = "General Sales Counter",
                    IsSalesPoint = true,
                    IsStockPoint = true,
                    TenantId = null
                },
                new UnitType
                {
                    Id = "type-warehouse-01",
                    Name = "Warehouse / Storage",
                    IsSalesPoint = false,
                    IsStockPoint = true,
                    TenantId = null
                },
                new UnitType
                {
                    Id = "type-kitchen-01",
                    Name = "Kitchen / Prep Area",
                    IsSalesPoint = false,
                    IsStockPoint = false,
                    TenantId = null
                }
            );
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(IMustHaveTenant).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(ApplicationDbContext)
                        .GetMethod(nameof(SetGlobalQueryFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        ?.MakeGenericMethod(entityType.ClrType);
                    method?.Invoke(this, new object[] { builder });
                }

                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(ApplicationDbContext)
                        .GetMethod(nameof(SetSoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        ?.MakeGenericMethod(entityType.ClrType);
                    method?.Invoke(this, new object[] { builder });
                }

            }
        }

        private void SetGlobalQueryFilter<T>(ModelBuilder builder) where T : class, IMustHaveTenant
        {
            // We resolve the tenant ID dynamically. If no scope exists (like in seeding), we return null (no filter).
            builder.Entity<T>().HasQueryFilter(e =>
                GetIdInternal() == null || e.TenantId == GetIdInternal());
        }

        private void SetSoftDeleteFilter<T>(ModelBuilder builder) where T : BaseEntity
        {
            builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
        }

        // Helper to safely get the Tenant ID without crashing the Root Provider
        private string? GetIdInternal()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                return scope.ServiceProvider.GetService<ITenantService>()?.GetTenantId();
            }
            catch { return null; }
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            using var scope = _serviceProvider.CreateScope();
            var tenantService = scope.ServiceProvider.GetService<ITenantService>();

            var currentTenantId = tenantService?.GetTenantId();
            var currentUserId = tenantService?.GetUserId();

            // 1. Audit Logging for BaseEntity
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = currentUserId;
                    entry.Entity.IsDeleted = false;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.LastModifiedAt = DateTime.UtcNow;
                    entry.Entity.LastModifiedBy = currentUserId;
                }
            }

            // 2. Multi-tenant Enforcement for IMustHaveTenant
            foreach (var entry in ChangeTracker.Entries<IMustHaveTenant>())
            {
                if (entry.State == EntityState.Added)
                {
                    // If the creator is a Tenant Admin, automatically stamp their ID.
                    // If the creator is a Super Admin (currentTenantId is null), 
                    // we respect whatever was manually set in the Dialog dropdown.
                    if (!string.IsNullOrEmpty(currentTenantId))
                    {
                        entry.Entity.TenantId = currentTenantId;
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    // Safeguard: Only enforce currentTenantId if it exists (Tenant Admin).
                    // This prevents a Super Admin from accidentally wiping an existing 
                    // TenantId during a general update.
                    if (!string.IsNullOrEmpty(currentTenantId))
                    {
                        entry.Entity.TenantId = currentTenantId;
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
