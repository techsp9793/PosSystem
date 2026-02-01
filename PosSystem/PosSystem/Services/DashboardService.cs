using PosSystem.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using PosSystem.Data;

namespace PosSystem.Services
{
    public class DashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDbContextFactory<LocalDbContext> _localDbFactory;

        public DashboardService(IUnitOfWork unitOfWork, IDbContextFactory<LocalDbContext> localDbFactory)
        {
            _unitOfWork = unitOfWork;
            _localDbFactory = localDbFactory;
        }

        public async Task<DashboardStats> GetDailyStatsAsync(DateTime date, string unitId)
        {
            var start = date.Date;
            var end = date.Date.AddDays(1).AddTicks(-1);

            // 1. Cloud Data (Filtered by Unit)
            var cloudOrders = await _unitOfWork.Orders.GetOrdersByDateRangeAsync(start, end);
            var filteredCloud = cloudOrders.Where(o => o.UnitId == unitId).ToList();

            // 2. Local Data (Filtered by Unit)
            // This is "Resilient" - it shows sales even if they haven't synced yet.
            using var localDb = await _localDbFactory.CreateDbContextAsync();
            var localOrders = await localDb.Orders
                .Where(o => o.UnitId == unitId && o.CreatedAt >= start && o.CreatedAt <= end)
                .ToListAsync();

            // 3. Merge (Dedup by ID)
            var allOrders = filteredCloud.Concat(localOrders)
                                         .GroupBy(o => o.Id)
                                         .Select(g => g.First())
                                         .ToList();

            return new DashboardStats
            {
                TotalRevenue = allOrders.Sum(o => o.TotalAmount),
                TotalOrders = allOrders.Count,
                AverageOrderValue = allOrders.Any() ? allOrders.Average(o => o.TotalAmount) : 0,
                CashSales = allOrders.Where(o => o.PaymentMethod == "Cash").Sum(o => o.TotalAmount),
                CardSales = allOrders.Where(o => o.PaymentMethod != "Cash").Sum(o => o.TotalAmount)
            };
        }
    }

    public class DashboardStats
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public decimal CashSales { get; set; }
        public decimal CardSales { get; set; }
    }
}