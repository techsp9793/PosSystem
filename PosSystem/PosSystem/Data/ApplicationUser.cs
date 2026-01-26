using Microsoft.AspNetCore.Identity;

namespace PosSystem.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? TenantId { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }

}
