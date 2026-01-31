using PosSystem.Data.Entities;
using PosSystem.Data.Repositories.Interfaces;
// We don't need Microsoft.EntityFrameworkCore here anymore because we rely on the Repository abstraction

namespace PosSystem.Services
{
    public class StockService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StockService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AdjustStockAsync(string unitId, string productId, long changeAmount, string type, string reason, string userId, string? referenceId = null)
        {
            // 1. Get the current stock record using FindAsync (Correct way)
            // Note: FindAsync returns IEnumerable, so we take the first one.
            var links = await _unitOfWork.UnitProducts.FindAsync(x => x.UnitId == unitId && x.ProductId == productId);
            var link = links.FirstOrDefault();

            if (link == null)
            {
                // Create if missing
                link = new UnitProduct
                {
                    Id = Guid.NewGuid().ToString(),
                    UnitId = unitId,
                    ProductId = productId,
                    TenantId = "tenant-1", // ideally fetch from context
                    StockQuantity = 0,
                    IsEnabled = true
                };
                await _unitOfWork.UnitProducts.AddAsync(link);
            }

            // 2. Calculate New Balance
            long oldBalance = link.StockQuantity ?? 0;
            long newBalance = oldBalance + changeAmount;
            link.StockQuantity = newBalance;

            // 3. Create Audit Log
            var log = new StockMovement
            {
                Id = Guid.NewGuid().ToString(),
                TenantId = link.TenantId ?? "tenant-1",
                UnitId = unitId,
                ProductId = productId,
                Type = type,
                QuantityChanged = changeAmount,
                StockAfter = newBalance,
                Reason = reason,
                UserId = userId,
                ReferenceId = referenceId,
                CreatedAt = DateTime.UtcNow
            };

            // [FIXED] Use the new Repository property we just added
            await _unitOfWork.StockMovements.AddAsync(log);

            // We do NOT call CompleteAsync here. 
            // The Dialog calls it at the end to save everything in one transaction.
        }
    }
}