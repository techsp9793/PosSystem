using System.ComponentModel.DataAnnotations;

namespace PosSystem.Data.Entities
{
    // Stores Key-Value pairs like "SMTP_Host" -> "smtp.gmail.com"
    public class SystemSetting:BaseEntity
    {
        [Required]
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        // Optional: If you want settings specific to a tenant, add TenantId. 
        // If null, it's a Global/SuperAdmin setting.
        public string? TenantId { get; set; }
    }
}