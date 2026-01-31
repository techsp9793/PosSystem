using System.ComponentModel.DataAnnotations.Schema;

namespace PosSystem.Data.Entities
{
    public class OrderItem : BaseEntity
    {
        public string OrderId { get; set; } = string.Empty;

        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty; // Snapshot name (in case product changes later)

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; } // Snapshot price

        public int Quantity { get; set; }
        public string TenantId { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice => UnitPrice * Quantity;
    }
}