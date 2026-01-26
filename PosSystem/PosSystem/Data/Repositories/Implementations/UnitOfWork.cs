using Microsoft.EntityFrameworkCore;
using PosSystem.Data.Entities;
using PosSystem.Data.Repositories.Interfaces;

namespace PosSystem.Data.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IRepository<Tenant>? _tenants;
        private IRepository<SubscriptionPlan>? _subscriptionPlans;
        private IRepository<SystemPermission>? _systemPermissions;
        private IRepository<PlanPermission>? _planPermissions;
        private IRepository<ApplicationRole>? _roles;
        private IRepository<ApplicationUser>? _users;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context =context;
        }

        public IRepository<Tenant> Tenants =>
            _tenants ??= new Repository<Tenant>(_context);

        public IRepository<SubscriptionPlan> SubscriptionPlans =>
            _subscriptionPlans ??= new Repository<SubscriptionPlan>(_context);
        public IRepository<SystemPermission> SystemPermissions =>
            _systemPermissions ??= new Repository<SystemPermission>(_context);

        // PlanPermissions (Now uses lazy loading same as above)
        public IRepository<PlanPermission> PlanPermissions =>
            _planPermissions ??= new Repository<PlanPermission>(_context);
        public IRepository<ApplicationRole> Roles => _roles ??= new Repository<ApplicationRole>(_context);
        public IRepository<ApplicationUser> Users => _users ??= new Repository<ApplicationUser>(_context);
        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}