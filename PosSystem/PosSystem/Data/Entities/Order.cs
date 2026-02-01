using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PosSystem.Data.Entities
{
    public class Order : BaseEntity
    {
        public string OrderNumber { get; set; } = string.Empty; // e.g. ORD-20231025-001
        public string UnitId { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;

        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }

        [Column(TypeName = "decimal(18,2)")] // Ensures database stores this as Money (2 decimal places)
        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; } = "Cash"; // Cash, UPI, Card
        public string Status { get; set; } = "Completed";   // Completed, Parked, Void

        public bool IsSynced { get; set; } = false;
        public DateTime? SyncedAt { get; set; }
        public string? PaymentReferenceId { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}