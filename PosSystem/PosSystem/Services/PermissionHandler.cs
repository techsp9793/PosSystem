//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
//using PosSystem.Data;
//using PosSystem.Data.Entities;
//using System.Security.Claims;

//namespace PosSystem.Services
//{
//    // The 'Requirement' is just a string marker (e.g., "Permissions.Products.View")
//    public class PermissionRequirement(string permission) : IAuthorizationRequirement
//    {
//        public string Permission { get; } = permission;
//    }

//    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
//    {
//        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

//        public PermissionHandler(IDbContextFactory<ApplicationDbContext> contextFactory)
//        {
//            _contextFactory = contextFactory;
//        }

//        protected override async Task HandleRequirementAsync(
//            AuthorizationHandlerContext context,
//            PermissionRequirement requirement)
//        {
//            if (context.User == null) return;

//            // [FIX]: Query the database directly instead of building a RoleManager
//            await using var dbContext = await _contextFactory.CreateDbContextAsync();

//            // 1. Get user roles from claims
//            var userRoles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

//            if (!userRoles.Any()) return;

//            // 2. Check if ANY of the user's roles have the required permission claim
//            // This is a much faster SQL query
//            var hasPermission = await (from role in dbContext.Roles
//                                       join claim in dbContext.RoleClaims on role.Id equals claim.RoleId
//                                       where userRoles.Contains(role.Name!)
//                                       && claim.ClaimType == "Permission"
//                                       && claim.ClaimValue == requirement.Permission
//                                       select claim).AnyAsync();

//            if (hasPermission)
//            {
//                context.Succeed(requirement);
//            }
//        }
//    }
//}
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PosSystem.Data;
using System.Security.Claims;

namespace PosSystem.Services
{
    public class PermissionRequirement(string permission) : IAuthorizationRequirement
    {
        public string Permission { get; } = permission;
    }

    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public PermissionHandler(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User?.Identity?.IsAuthenticated != true) return;

            // Query the Database DIRECTLY. 
            // This bypasses the need for RoleManager and ITenantService entirely.
            await using var dbContext = await _contextFactory.CreateDbContextAsync();

            var userRoles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            if (!userRoles.Any()) return;

            // We check the RoleClaims table directly for the "Permission" claim type
            var hasPermission = await (from role in dbContext.Roles
                                       join claim in dbContext.RoleClaims on role.Id equals claim.RoleId
                                       where userRoles.Contains(role.Name)
                                       && claim.ClaimType == "Permission"
                                       && claim.ClaimValue == requirement.Permission
                                       select claim).AnyAsync();

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
}