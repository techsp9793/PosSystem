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
        Task<int> CompleteAsync();
    }
}