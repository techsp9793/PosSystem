using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PosSystem.Data.Entities
{
    // FIX: Must inherit from BaseEntity to get 'Id'
    public class SubscriptionPlan : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public PlanType Type { get; set; } = PlanType.Monthly; // [NEW]
        public int DurationInDays { get; set; }
        public string? Features { get; set; }
        public bool IsActive { get; set; } = true;
    }
    public enum PlanType { Monthly, Yearly, OneTime }
}