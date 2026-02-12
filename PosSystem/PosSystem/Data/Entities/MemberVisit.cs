using System.ComponentModel.DataAnnotations;
using PosSystem.Data.Entities.Interfaces;

namespace PosSystem.Data.Entities
{
    public class MemberVisit : BaseEntity, IMustHaveTenant
    {
        [Required]
        public string MemberId { get; set; } = string.Empty;
        public virtual Member? Member { get; set; }

        public DateTime VisitTime { get; set; } = DateTime.UtcNow;
        public string EntryGate { get; set; } = "Main Gate"; // e.g., "North Gate"

        public string TenantId { get; set; } = string.Empty;
    }
}