//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Options;
//using PosSystem.Data;
//using PosSystem.Data.Entities;
//using System.Security.Claims;

//namespace PosSystem.Services
//{
//    public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
//    {
//        public CustomUserClaimsPrincipalFactory(
//            UserManager<ApplicationUser> userManager,
//            RoleManager<ApplicationRole> roleManager,
//            IOptions<IdentityOptions> optionsAccessor)
//            : base(userManager, roleManager, optionsAccessor)
//        {
//        }

//        //protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
//        //{
//        //    var identity = await base.GenerateClaimsAsync(user);

//        //    // Now this works because ApplicationUser has TenantId
//        //    if (!string.IsNullOrEmpty(user.TenantId))
//        //    {
//        //        identity.AddClaim(new Claim("tenant_id", user.TenantId));
//        //    }

//        //    return identity;
//        //}
//    }
//}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PosSystem.Data;
using PosSystem.Data.Entities;
using System.Security.Claims;

namespace PosSystem.Services
{
    public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public CustomUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor,
            IDbContextFactory<ApplicationDbContext> contextFactory)
            : base(userManager, roleManager, optionsAccessor)
        {
            _contextFactory = contextFactory;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            // 1. Generate base claims (Username, Email, etc.)
            // Note: This might miss the Global Role if the DbContext filter is active, 
            // so we will manually ensure roles are added below.
            var identity = await base.GenerateClaimsAsync(user);

            // 2. Add TenantId
            if (!string.IsNullOrEmpty(user.TenantId))
            {
                identity.AddClaim(new Claim("tenant_id", user.TenantId));
            }

            // 3. INTELLIGENT FIX: Fetch Roles & Permissions ignoring Query Filters
            // This ensures we find "Global Roles" (TenantId = NULL) even if the user is in a Tenant.
            await using var dbContext = await _contextFactory.CreateDbContextAsync();

            var userRolesData = await (from ur in dbContext.UserRoles
                                       join r in dbContext.Roles.IgnoreQueryFilters() on ur.RoleId equals r.Id
                                       where ur.UserId == user.Id
                                       select new { r.Name, r.Id }).ToListAsync();

            foreach (var role in userRolesData)
            {
                // A. Ensure the Role Name claim exists (Fixes the "Missing Role" issue)
                if (!identity.HasClaim(Options.ClaimsIdentity.RoleClaimType, role.Name!))
                {
                    identity.AddClaim(new Claim(Options.ClaimsIdentity.RoleClaimType, role.Name!));
                }

                // B. Fetch Permissions for this Role (Fixes the "Hidden Menu" issue)
                var permissions = await dbContext.RoleClaims
                    .Where(rc => rc.RoleId == role.Id && rc.ClaimType == "Permission")
                    .Select(rc => rc.ClaimValue)
                    .ToListAsync();

                foreach (var permission in permissions)
                {
                    if (permission != null && !identity.HasClaim("Permission", permission))
                    {
                        identity.AddClaim(new Claim("Permission", permission));
                    }
                }
            }

            return identity;
        }
    }
}