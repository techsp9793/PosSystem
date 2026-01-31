using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PosSystem.Data.Entities
{
    public class UnitProduct : BaseEntity
    {
        // [FIX] Added TenantId to solve the CS0117 error
        [Required]
        public string TenantId { get; set; } = string.Empty;

        [Required]
        public string UnitId { get; set; } = string.Empty;

        [ForeignKey("UnitId")]
        public virtual Unit? Unit { get; set; }

        [Required]
        public string ProductId { get; set; } = string.Empty;

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        public bool IsEnabled { get; set; } = true;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PriceOverride { get; set; }

        public long? StockQuantity { get; set; } = 0;
    }
}