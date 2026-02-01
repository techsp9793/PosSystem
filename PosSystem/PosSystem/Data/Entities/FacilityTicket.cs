using PosSystem.Data.Entities.Interfaces;

namespace PosSystem.Data.Entities
{
    public class FacilityTicket : BaseEntity, IMustHaveTenant
    {
        public string TicketCode { get; set; } = string.Empty; // Encrypted Blob
        public string OrderId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public Product? Product { get; set; }

        public bool IsUsed { get; set; } = false;
        public DateTime? UsedAt { get; set; }
        public DateTime ExpiryDate { get; set; }

        public string TenantId { get; set; } = string.Empty;
    }
}