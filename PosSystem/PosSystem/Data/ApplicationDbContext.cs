//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
//using PosSystem.Data.Entities;
//using PosSystem.Data.Entities.Interfaces;
//using PosSystem.Services;

//namespace PosSystem.Data
//{
//    public class ApplicationDbContext:IdentityDbContext<ApplicationUser, ApplicationRole, string>
//    {
//        private readonly ITenantService? _tenantService;

//        public ApplicationDbContext(
//            DbContextOptions<ApplicationDbContext> options,
//            IServiceProvider serviceProvider) // Use IServiceProvider to resolve services safely
//            : base(options)
//        {
//            // Try to get the service, but don't crash if it's not there
//            _tenantService = serviceProvider.GetService<ITenantService>();
//        }
//        // 1. Register the Table
//        public DbSet<Tenant> Tenants { get; set; }
//        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
//        public DbSet<SystemPermission> SystemPermissions { get; set; }
//        public DbSet<PlanPermission> PlanPermissions { get; set; }
//        protected override void OnModelCreating(ModelBuilder builder)
//        {
//            base.OnModelCreating(builder);

//            // 2. Global Filters (Multitenancy & Soft Delete)
//            foreach (var entityType in builder.Model.GetEntityTypes())
//            {
//                if (typeof(IMustHaveTenant).IsAssignableFrom(entityType.ClrType))
//                {
//                    var method = typeof(ApplicationDbContext)
//                        .GetMethod(nameof(SetGlobalQueryFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
//                        ?.MakeGenericMethod(entityType.ClrType);
//                    method?.Invoke(this, new object[] { builder });
//                }

//                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
//                {
//                    var method = typeof(ApplicationDbContext)
//                        .GetMethod(nameof(SetSoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
//                        ?.MakeGenericMethod(entityType.ClrType);
//                    method?.Invoke(this, new object[] { builder });
//                }
//            }
//        }

//        private void SetGlobalQueryFilter<T>(ModelBuilder builder) where T : class, IMustHaveTenant
//        {
//            //builder.Entity<T>().HasQueryFilter(e => e.TenantId == _tenantService.GetTenantId());
//            builder.Entity<T>().HasQueryFilter(e =>
//            _tenantService == null || e.TenantId == _tenantService.GetTenantId());
//        }

//        private void SetSoftDeleteFilter<T>(ModelBuilder builder) where T : BaseEntity
//        {
//            builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
//        }

//        // 3. Auto-Save Logic (Audit + TenantId)
//        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
//        {
//            var currentTenantId = _tenantService.GetTenantId();
//            var currentUserId = _tenantService.GetUserId();

//            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
//            {
//                switch (entry.State)
//                {
//                    case EntityState.Added:
//                        entry.Entity.CreatedAt = DateTime.UtcNow;
//                        entry.Entity.CreatedBy = currentUserId;
//                        entry.Entity.IsDeleted = false;
//                        break;
//                    case EntityState.Modified:
//                        entry.Entity.LastModifiedAt = DateTime.UtcNow;
//                        entry.Entity.LastModifiedBy = currentUserId;
//                        break;
//                }
//            }

//            foreach (var entry in ChangeTracker.Entries<IMustHaveTenant>())
//            {
//                if (entry.State == EntityState.Added && !string.IsNullOrEmpty(currentTenantId))
//                {
//                    entry.Entity.TenantId = currentTenantId;
//                }
//            }

//            return base.SaveChangesAsync(cancellationToken);
//        }
//    }
//}
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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
            // Resolve services only when Saving
            using var scope = _serviceProvider.CreateScope();
            var tenantService = scope.ServiceProvider.GetService<ITenantService>();

            var currentTenantId = tenantService?.GetTenantId();
            var currentUserId = tenantService?.GetUserId();

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

            foreach (var entry in ChangeTracker.Entries<IMustHaveTenant>())
            {
                if (entry.State == EntityState.Added && !string.IsNullOrEmpty(currentTenantId))
                {
                    entry.Entity.TenantId = currentTenantId;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
