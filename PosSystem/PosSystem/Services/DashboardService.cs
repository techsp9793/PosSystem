using Microsoft.EntityFrameworkCore;
using PosSystem.Data;
using PosSystem.Data.Entities;

namespace PosSystem.Services
{
    public class DashboardService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public DashboardService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<DashboardStats> GetStatsAsync(string? tenantId)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            var today = DateTime.UtcNow.Date;

            // 1. Sales Stats (Today)
            var todaysOrders = await db.Orders
                .Where(o => o.TenantId == tenantId && o.CreatedAt >= today && o.Status != "Cancelled")
                .ToListAsync();

            decimal dailyRevenue = todaysOrders.Sum(o => o.TotalAmount);
            int orderCount = todaysOrders.Count;

            // 2. Member Stats
            int activeMembers = await db.Members
                .Where(m => m.TenantId == tenantId && m.IsActive)
                .CountAsync();

            int todaysVisits = await db.MemberVisits
                .Where(v => v.TenantId == tenantId && v.VisitTime >= today)
                .CountAsync();

            // 3. Inventory Alerts
            int lowStockCount = await db.Products
                .Where(p => p.TenantId == tenantId && p.TrackStock && p.StockQuantity <= 10)
                .CountAsync();

            return new DashboardStats
            {
                DailyRevenue = dailyRevenue,
                DailyOrders = orderCount,
                ActiveMembers = activeMembers,
                TodaysVisits = todaysVisits,
                LowStockItems = lowStockCount
            };
        }

        public async Task<List<double>> GetWeeklySalesDataAsync(string? tenantId)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            var last7Days = DateTime.UtcNow.Date.AddDays(-6);

            var salesData = await db.Orders
                .Where(o => o.TenantId == tenantId && o.CreatedAt >= last7Days && o.Status != "Cancelled")
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(o => o.TotalAmount) })
                .ToListAsync();

            // Fill in missing days with 0
            var result = new List<double>();
            for (int i = 0; i < 7; i++)
            {
                var dateToCheck = last7Days.AddDays(i);
                var entry = salesData.FirstOrDefault(s => s.Date == dateToCheck);
                result.Add(entry != null ? (double)entry.Total : 0);
            }

            return result;
        }
    }

    public class DashboardStats
    {
        public decimal DailyRevenue { get; set; }
        public int DailyOrders { get; set; }
        public int ActiveMembers { get; set; }
        public int TodaysVisits { get; set; }
        public int LowStockItems { get; set; }
    }
}