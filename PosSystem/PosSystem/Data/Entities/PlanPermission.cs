using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PosSystem.Data.Entities
{
    public class PlanPermission
    {
        [Key]
        public int Id { get; set; }

        public string SubscriptionPlanId { get; set; } = string.Empty;

        [ForeignKey("SubscriptionPlanId")]
        public SubscriptionPlan? SubscriptionPlan { get; set; }

        public string PermissionCode { get; set; } = string.Empty; // e.g. "Permissions.Products.Create"
    }
}