using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PosSystem.Data.Entities
{
    public class Unit : BaseEntity
    {
        [Required]
        public string TenantId { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty; // "North Gate"

        [Required]
        public string UnitTypeId { get; set; } = string.Empty;

        [ForeignKey("UnitTypeId")]
        public virtual UnitType? UnitType { get; set; }

        public bool IsActive { get; set; } = true;
    }
}