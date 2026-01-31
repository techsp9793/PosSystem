using Microsoft.AspNetCore.Identity;
using PosSystem.Data.Entities.Interfaces;

namespace PosSystem.Data
{
    public class ApplicationUser : IdentityUser, IMustHaveTenant
    {
        public string? TenantId { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? DefaultUnitId { get; set; }
    }

}
