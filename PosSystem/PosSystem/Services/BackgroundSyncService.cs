using Microsoft.EntityFrameworkCore;
using PosSystem.Data;
using PosSystem.Data.Entities;

namespace PosSystem.Services
{
    public class BackgroundSyncService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BackgroundSyncService> _logger;

        public BackgroundSyncService(IServiceScopeFactory scopeFactory, ILogger<BackgroundSyncService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SyncPendingOrders();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Sync failed. Retrying in 10 seconds...");
                }
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private async Task SyncPendingOrders()
        {
            using var scope = _scopeFactory.CreateScope();

            // 1. Get Pending Orders from Local SQLite
            var localDbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<LocalDbContext>>();
            using var localDb = localDbFactory.CreateDbContext();

            var pendingOrders = await localDb.Orders
                .Include(o => o.OrderItems)
                .Where(o => !o.IsSynced)
                .ToListAsync();

            if (!pendingOrders.Any()) return;

            // 2. Push to Cloud SQL Server
            var cloudDb = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            foreach (var localOrder in pendingOrders)
            {
                // Check duplication
                bool exists = await cloudDb.Orders.AnyAsync(o => o.Id == localOrder.Id);
                if (!exists)
                {
                    var cloudOrder = new Order
                    {
                        Id = localOrder.Id,
                        OrderNumber = localOrder.OrderNumber,
                        TotalAmount = localOrder.TotalAmount,
                        PaymentMethod = localOrder.PaymentMethod,
                        Status = localOrder.Status,
                        CreatedAt = localOrder.CreatedAt,
                        UnitId = localOrder.UnitId,
                        TenantId = localOrder.TenantId,
                        CustomerName = localOrder.CustomerName,
                        CustomerPhone = localOrder.CustomerPhone,
                        IsSynced = true,
                        SyncedAt = DateTime.UtcNow
                    };

                    foreach (var item in localOrder.OrderItems)
                    {
                        cloudOrder.OrderItems.Add(new OrderItem
                        {
                            Id = item.Id, // Keep ID consistent
                            ProductId = item.ProductId,
                            ProductName = item.ProductName,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            // TotalPrice is calculated automatically by the entity
                            TenantId = localOrder.TenantId
                        });
                    }
                    cloudDb.Orders.Add(cloudOrder);
                }

                // 3. Update Local Status
                localOrder.IsSynced = true;
                localOrder.SyncedAt = DateTime.UtcNow;
            }

            await cloudDb.SaveChangesAsync();
            await localDb.SaveChangesAsync();

            _logger.LogInformation($"Synced {pendingOrders.Count} orders.");
        }
    }
}