using System.ComponentModel.DataAnnotations;

namespace PosSystem.Data.Entities
{
    public class SystemPermission
    {
        [Key]
        public string PermissionCode { get; set; } = string.Empty; // PK: "Permissions.Products.Create"

        public string GroupName { get; set; } = string.Empty; // "Products"
        public string Name { get; set; } = string.Empty;      // "Create"
    }
}
