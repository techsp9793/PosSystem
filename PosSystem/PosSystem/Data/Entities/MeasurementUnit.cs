using System.ComponentModel.DataAnnotations;
using PosSystem.Data.Entities.Interfaces;

namespace PosSystem.Data.Entities
{
    public class MeasurementUnit : BaseEntity, IMustHaveTenant
    {
        [Required]
        public string Name { get; set; } = string.Empty; // e.g., "Person", "Kilogram", "Hour"

        [Required]
        public string Symbol { get; set; } = string.Empty; // e.g., "pax", "kg", "hr"

        public string TenantId { get; set; } = string.Empty;
    }
}