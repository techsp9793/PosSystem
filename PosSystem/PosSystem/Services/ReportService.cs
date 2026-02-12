using Microsoft.EntityFrameworkCore;
using PosSystem.Data;
using PosSystem.Data.Entities;
using System.Text;

namespace PosSystem.Services
{
    public class ReportService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public ReportService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        // --- 1. SALES REPORT ---
        public async Task<List<Order>> GetSalesReportAsync(string tenantId, DateTime start, DateTime end)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            var endDate = end.Date.AddDays(1).AddTicks(-1);

            return await db.Orders
                .Where(o => o.TenantId == tenantId
                            && o.CreatedAt >= start.Date
                            && o.CreatedAt <= endDate
                            && o.Status != "Cancelled")
                // FIX 1: Changed 'Items' to 'OrderItems'
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        // --- 2. PRODUCT PERFORMANCE ---
        public async Task<List<ProductStats>> GetProductPerformanceAsync(string tenantId, DateTime start, DateTime end)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            var endDate = end.Date.AddDays(1).AddTicks(-1);

            return await db.OrderItems
                .Where(i => i.Order.TenantId == tenantId
                            && i.Order.CreatedAt >= start.Date
                            && i.Order.CreatedAt <= endDate
                            && i.Order.Status != "Cancelled")
                .GroupBy(i => new { i.ProductId, i.ProductName })
                .Select(g => new ProductStats
                {
                    ProductName = g.Key.ProductName,
                    QuantitySold = g.Sum(x => x.Quantity),
                    // FIX 2: Calculate Total manually (Quantity * UnitPrice)
                    TotalRevenue = g.Sum(x => x.Quantity * x.UnitPrice)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToListAsync();
        }

        // --- 3. INVENTORY VALUATION ---
        public async Task<List<StockStats>> GetStockValuationAsync(string tenantId)
        {
            using var db = await _dbFactory.CreateDbContextAsync();

            return await db.Products
                .Where(p => p.TenantId == tenantId && p.TrackStock)
                .Select(p => new StockStats
                {
                    ProductName = p.Name,
                    CurrentStock = p.StockQuantity,
                    SellingPrice = p.BasePrice,
                    PotentialValue = p.StockQuantity * p.BasePrice,
                    Category = p.Category.Name
                })
                .OrderBy(p => p.CurrentStock)
                .ToListAsync();
        }

        // --- 4. MEMBER ACTIVITY ---
        public async Task<List<MemberVisit>> GetMemberVisitsAsync(string tenantId, DateTime start, DateTime end)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            var endDate = end.Date.AddDays(1).AddTicks(-1);

            return await db.MemberVisits
                .Include(v => v.Member)
                .Where(v => v.TenantId == tenantId
                            && v.VisitTime >= start.Date
                            && v.VisitTime <= endDate)
                .OrderByDescending(v => v.VisitTime)
                .ToListAsync();
        }

        // --- HELPER: EXPORT TO CSV ---
        public byte[] GenerateCsv<T>(IEnumerable<T> data)
        {
            var sb = new StringBuilder();
            var properties = typeof(T).GetProperties();

            // Header
            sb.AppendLine(string.Join(",", properties.Select(p => p.Name)));

            // Rows
            foreach (var item in data)
            {
                var values = properties.Select(p =>
                {
                    var val = p.GetValue(item, null);
                    var str = val?.ToString() ?? "";
                    if (str.Contains(",")) return $"\"{str}\"";
                    return str;
                });
                sb.AppendLine(string.Join(",", values));
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }

    public class ProductStats
    {
        public string ProductName { get; set; } = "";
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class StockStats
    {
        public string ProductName { get; set; } = "";
        public string Category { get; set; } = "";
        public int CurrentStock { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal PotentialValue { get; set; }
    }
}