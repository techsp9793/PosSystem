using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PosSystem.Data.Entities
{
    public class StockMovement : BaseEntity
    {
        [Required]
        public string TenantId { get; set; } = string.Empty;

        [Required]
        public string UnitId { get; set; } = string.Empty; // Which store?

        [Required]
        public string ProductId { get; set; } = string.Empty;

        // "Sale", "Adjustment", "Purchase", "Return", "Transfer"
        [Required]
        public string Type { get; set; } = "Adjustment";

        public long QuantityChanged { get; set; } // e.g. -5 or +50

        public long StockAfter { get; set; } // Snapshot of balance after change

        public string? ReferenceId { get; set; } // e.g. OrderId or AdjustmentId

        public string? Reason { get; set; } // "Damaged", "Sold", "Weekly Restock"

        public string? UserId { get; set; } // Who did it?

        // Navigation
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}