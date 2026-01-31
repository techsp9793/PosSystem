using Microsoft.EntityFrameworkCore;
using PosSystem.Data.Entities;
using PosSystem.Data.Repositories.Interfaces;
using static PosSystem.Permissions.AppPermissions;

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
        private IRepository<ItemCategory>? _categories;
        private IRepository<Product>? _products;
        private IRepository<UnitProduct>? _unitProducts;
        private IRepository<Unit>? _units;
        private IRepository<UnitType>? _unitTypes;
        private IOrderRepository? _orders;

        // [NEW] Backing Field for StockMovements
        private IRepository<StockMovement>? _stockMovements;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IRepository<Tenant> Tenants =>
            _tenants ??= new Repository<Tenant>(_context);

        public IRepository<SubscriptionPlan> SubscriptionPlans =>
            _subscriptionPlans ??= new Repository<SubscriptionPlan>(_context);

        public IRepository<SystemPermission> SystemPermissions =>
            _systemPermissions ??= new Repository<SystemPermission>(_context);

        public IRepository<PlanPermission> PlanPermissions =>
            _planPermissions ??= new Repository<PlanPermission>(_context);

        public IRepository<ApplicationRole> Roles =>
            _roles ??= new Repository<ApplicationRole>(_context);

        public IRepository<ApplicationUser> Users =>
            _users ??= new Repository<ApplicationUser>(_context);

        public IRepository<ItemCategory> Categories =>
            _categories ??= new Repository<ItemCategory>(_context);

        public IRepository<Product> Products =>
            _products ??= new Repository<Product>(_context);

        public IRepository<UnitProduct> UnitProducts =>
            _unitProducts ??= new Repository<UnitProduct>(_context);

        public IRepository<Unit> Units =>
            _units ??= new Repository<Unit>(_context);

        public IRepository<UnitType> UnitTypes =>
            _unitTypes ??= new Repository<UnitType>(_context);

        public IOrderRepository Orders =>
            _orders ??= new OrderRepository(_context);

        // [NEW] Public Property Implementation
        public IRepository<StockMovement> StockMovements =>
            _stockMovements ??= new Repository<StockMovement>(_context);

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}