using Microsoft.EntityFrameworkCore;
using PosSystem.Data.Entities;
using PosSystem.Data.Repositories.Interfaces;

namespace PosSystem.Data.Repositories.Implementations
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        // FIX: Ensure 'string id' is in the parameter list
        public async Task<Order?> GetOrderWithDetailsAsync(string id)
        {
            return await _context.Orders
                                 .Include(o => o.OrderItems)
                                 .FirstOrDefaultAsync(o => o.Id == id); // Now 'id' matches the parameter
        }
        public async Task<List<Order>> GetOrdersByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _context.Orders
                .Where(o => o.CreatedAt >= start && o.CreatedAt <= end)
                .Where(o => o.Status != "Void") // Exclude voided orders automatically
                .ToListAsync();
        }
    }
}