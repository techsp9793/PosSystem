using System.ComponentModel.DataAnnotations;

namespace PosSystem.Data.Entities
{
    public class UnitType : BaseEntity
    {
        // If Null, it's a System Default (e.g., "Warehouse"). 
        // If set, it's a specific Tenant's custom type.
        public string? TenantId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        // [FIX] Added Description to resolve build errors
        public string? Description { get; set; }

        public bool IsSalesPoint { get; set; } = true; // Can allow login/POS?
        public bool IsStockPoint { get; set; } = true; // Can hold inventory?
    }
}