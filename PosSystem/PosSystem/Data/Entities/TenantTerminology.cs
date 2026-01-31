using System.ComponentModel.DataAnnotations;

namespace PosSystem.Data.Entities
{
    public class TenantTerminology : BaseEntity
    {
        [Required]
        public string TenantId { get; set; } = string.Empty;

        [Required]
        public string TermKey { get; set; } = string.Empty; // e.g., "Label_Product"

        [Required]
        public string TermValue { get; set; } = string.Empty; // e.g., "Pass"
    }
}