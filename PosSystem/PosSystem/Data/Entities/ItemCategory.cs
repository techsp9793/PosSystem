using System.ComponentModel.DataAnnotations;

namespace PosSystem.Data.Entities
{
    public class ItemCategory : BaseEntity
    {
        [Required]
        public string TenantId { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty; // e.g., "Water Rides"

        public string? ColorCode { get; set; } = "#2196F3"; // For POS UI buttons

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}