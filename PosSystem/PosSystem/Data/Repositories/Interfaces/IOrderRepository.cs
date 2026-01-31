using PosSystem.Data.Entities;

namespace PosSystem.Data.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        // We can add specific reporting methods here later, e.g.:
        Task<Order?> GetOrderWithDetailsAsync(string id);
        Task<List<Order>> GetOrdersByDateRangeAsync(DateTime start, DateTime end);
    }
}