using PosSystem.Data.Repositories.Interfaces;

namespace PosSystem.Services
{
    public class DashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardStats> GetDailyStatsAsync(DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = date.Date.AddDays(1).AddTicks(-1);

            // [FIXED] Use the repository method we just created
            var ordersToday = await _unitOfWork.Orders.GetOrdersByDateRangeAsync(startOfDay, endOfDay);

            return new DashboardStats
            {
                TotalRevenue = ordersToday.Sum(o => o.TotalAmount),
                TotalOrders = ordersToday.Count,
                AverageOrderValue = ordersToday.Any() ? ordersToday.Average(o => o.TotalAmount) : 0,
                CashSales = ordersToday.Where(o => o.PaymentMethod == "Cash").Sum(o => o.TotalAmount),
                CardSales = ordersToday.Where(o => o.PaymentMethod != "Cash").Sum(o => o.TotalAmount)
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