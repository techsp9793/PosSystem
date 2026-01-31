using System.ComponentModel.DataAnnotations;

namespace PosSystem.Data.Entities
{
    public class BusinessCategory : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty; // e.g., "Amusement Park"

        public string Description { get; set; } = string.Empty;

        // Defaults for Terminology (Seeds the tenant's config)
        public string DefaultProductTerm { get; set; } = "Product"; // e.g., "Ticket"
        public string DefaultCategoryTerm { get; set; } = "Category"; // e.g., "Zone"
    }
}