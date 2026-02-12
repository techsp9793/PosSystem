using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PosSystem.Data.Entities
{
    public class Product : BaseEntity
    {
        [Required]
        public string TenantId { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty; // e.g., "Adult Entry"

        public string? SKU { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }

        [Required]
        public string CategoryId { get; set; } = string.Empty;

        [ForeignKey("CategoryId")]
        public virtual ItemCategory? Category { get; set; }

        // --- NEW INVENTORY FLAGS ---

        // If TRUE: Appears on the Cashier's screen (Menu/Catalog).
        // If FALSE: Only visible in Admin (e.g., Office Supplies, Raw Ingredients).
        public bool IsPosVisible { get; set; } = true;

        // If TRUE: We count how many are left.
        public bool TrackStock { get; set; } = false;

        // The actual count (only relevant if TrackStock is true).
        public int StockQuantity { get; set; } = 0;
        public string? MeasurementUnitId { get; set; }

        [ForeignKey("MeasurementUnitId")]
        public virtual MeasurementUnit? MeasurementUnit { get; set; }
        public virtual ICollection<UnitProduct> UnitAvailability { get; set; } = new List<UnitProduct>();
    }
}