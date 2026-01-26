using Microsoft.AspNetCore.Identity;
using PosSystem.Data.Entities.Interfaces;

namespace PosSystem.Data.Entities
{
    // Inherits IdentityRole but adds TenantId
    public class ApplicationRole : IdentityRole, IMustHaveTenant
    {
        public string? TenantId { get; set; }
        public string? Description { get; set; }

        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName, string? tenantId = null) : base(roleName)
        {
            TenantId = tenantId;
        }
    }
}