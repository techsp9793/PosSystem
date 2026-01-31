using PosSystem.Data.Entities;

namespace PosSystem.Data.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Add these two properties so the pages can see them
        IRepository<Tenant> Tenants { get; }
        IRepository<SubscriptionPlan> SubscriptionPlans { get; }
        IRepository<SystemPermission> SystemPermissions { get; }
        IRepository<PlanPermission> PlanPermissions { get; }
        IRepository<ApplicationRole> Roles { get; }
        IRepository<ApplicationUser> Users { get; }
        IRepository<ItemCategory> Categories { get; }
        IRepository<Product> Products { get; }
        IRepository<UnitProduct> UnitProducts { get; }
        IRepository<Unit> Units { get; }
        IRepository<UnitType> UnitTypes { get; }
        IOrderRepository Orders { get; }
        IRepository<StockMovement> StockMovements { get; }
        Task<int> CompleteAsync();
    }
}