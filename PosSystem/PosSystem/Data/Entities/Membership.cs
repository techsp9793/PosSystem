using PosSystem.Data.Entities.Interfaces;

namespace PosSystem.Data.Entities
{
    public class Membership : BaseEntity, IMustHaveTenant
    {
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public string MembershipNumber { get; set; } = string.Empty;
        public string Tier { get; set; } = "Silver"; // Silver, Gold, etc.
        public DateTime ExpiryDate { get; set; }

        public string TenantId { get; set; } = string.Empty;
    }
}