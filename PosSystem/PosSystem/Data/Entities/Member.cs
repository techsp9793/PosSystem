using System.ComponentModel.DataAnnotations;
using PosSystem.Data.Entities.Interfaces;

namespace PosSystem.Data.Entities
{
    public class Member : BaseEntity, IMustHaveTenant
    {
        [Required]
        public string Name { get; set; } = string.Empty; // Mandatory

        public string? PhoneNumber { get; set; } // Optional
        public string? Email { get; set; }       // Optional
        public string? PhotoUrl { get; set; }    // Optional (For Gatekeeper verification)

        // --- Membership Status ---
        public DateTime JoinDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiryDate { get; set; } // Null = Lifetime

        public string MembershipType { get; set; } = "Standard"; // e.g., "Monthly", "Yearly"
        public bool IsActive { get; set; } = true;

        // --- Security ---
        // The QR Code contains THIS Token, not the ID. 
        // Allows us to regenerate the QR without deleting the member.
        [Required]
        public string QrToken { get; set; } = Guid.NewGuid().ToString();

        // --- Notifications (Future Proofing) ---
        public bool OptInSms { get; set; } = false;
        public bool OptInEmail { get; set; } = false;
        public bool OptInWhatsApp { get; set; } = false;

        public string TenantId { get; set; } = string.Empty;
    }
}