using System.ComponentModel.DataAnnotations;

namespace PosSystem.Data.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }

        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }

        public bool IsDeleted { get; set; } = false; // Soft Delete support
    }
}
