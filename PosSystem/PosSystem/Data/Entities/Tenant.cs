using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PosSystem.Data.Entities
{
    public class Tenant : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? SubscriptionPlanId { get; set; }

        [ForeignKey("SubscriptionPlanId")]
        public SubscriptionPlan? SubscriptionPlan { get; set; }

        public string? PlanType { get; set; }

        public bool IsActive { get; set; } = true;
        public string? BusinessCategoryId { get; set; }

        [ForeignKey("BusinessCategoryId")]
        public virtual BusinessCategory? BusinessCategory { get; set; }

        public string? CurrencySymbol { get; set; } = "$";
        public string? TimeZone { get; set; } = "UTC";

        // Navigation to their Units (Branches)
        public virtual ICollection<Unit> Units { get; set; } = new List<Unit>();
    }
}
