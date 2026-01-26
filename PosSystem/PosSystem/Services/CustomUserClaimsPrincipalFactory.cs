using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PosSystem.Data;
using PosSystem.Data.Entities;
using System.Security.Claims;

namespace PosSystem.Services
{
    public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        public CustomUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            // Now this works because ApplicationUser has TenantId
            if (!string.IsNullOrEmpty(user.TenantId))
            {
                identity.AddClaim(new Claim("tenant_id", user.TenantId));
            }

            return identity;
        }
    }
}