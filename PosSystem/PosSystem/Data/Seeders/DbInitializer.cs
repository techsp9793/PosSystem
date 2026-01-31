using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PosSystem.Data.Entities;
using System.Security.Claims;
using PosSystem.Permissions; // Required for AppPermissions

namespace PosSystem.Data.Seeders
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndPermissionsAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var config = serviceProvider.GetRequiredService<IConfiguration>();

            // --- 0. ENSURE PERMISSIONS EXIST IN DB FIRST ---
            await PermissionSeeder.SeedPermissionsAsync(serviceProvider);

            // Read from AppSettings
            string roleName = config["SuperAdmin:Role"] ?? "Techspruce";
            string superAdminEmail = config["SuperAdmin:Email"] ?? "admin@local";
            string defaultPassword = config["SuperAdmin:Password"] ?? "Password123!";

            // --- 2. Create the Super Admin Role ---
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new ApplicationRole
                {
                    Name = roleName,
                    Description = "Global Super Admin Role",
                    TenantId = null
                };
                await roleManager.CreateAsync(role);
            }

            // --- 3. Sync Super Admin Permissions ---
            var techSpruceRole = await roleManager.FindByNameAsync(roleName);
            if (techSpruceRole == null) throw new Exception("Role not found!");

            var allPermissions = await context.SystemPermissions.ToListAsync();
            var existingClaims = await roleManager.GetClaimsAsync(techSpruceRole);
            var existingCodes = existingClaims
                                .Where(c => c.Type == "Permission")
                                .Select(c => c.Value)
                                .ToHashSet();

            foreach (var perm in allPermissions)
            {
                if (!existingCodes.Contains(perm.PermissionCode))
                {
                    await roleManager.AddClaimAsync(techSpruceRole, new Claim("Permission", perm.PermissionCode));
                }
            }

            // --- 4. Create Super Admin User ---
            var superUser = await userManager.FindByEmailAsync(superAdminEmail);
            if (superUser == null)
            {
                superUser = new ApplicationUser
                {
                    UserName = superAdminEmail,
                    Email = superAdminEmail,
                    EmailConfirmed = true,
                    TenantId = null
                };
                var userResult = await userManager.CreateAsync(superUser, defaultPassword);
                if (!userResult.Succeeded) throw new Exception("Failed to create Super Admin");
            }

            // --- 5. Assign User to Role ---
            if (!await userManager.IsInRoleAsync(superUser, roleName))
            {
                await userManager.AddToRoleAsync(superUser, roleName);
            }


            // =========================================================
            // --- [NEW] STEP 6: SYNC TENANT ADMIN / MANAGER ROLE ---
            // =========================================================
            // We search for your standard tenant role (likely named "Admin" or "Manager")
            // and forcefully inject the POS permissions so they can access the new module.

            string tenantRoleName = "Admin"; // Change this to "Manager" if your role is named Manager

            if (await roleManager.RoleExistsAsync(tenantRoleName))
            {
                // Find all roles with this name (across different tenants if needed, or the global template)
                // Since ASP.NET Identity roles are unique by Name (unless normalized), 
                // we usually find the specific one. If you have multiple tenants with same role name,
                // you might need to iterate. For now, we assume one standard Admin role or we loop.

                // If you are using multi-tenant roles (where each tenant has their own "Admin" role),
                // we should loop through ALL roles named "Admin".
                var targetRoles = roleManager.Roles.Where(r => r.Name == tenantRoleName).ToList();

                foreach (var targetRole in targetRoles)
                {
                    // 1. Get current permissions
                    var currentClaims = await roleManager.GetClaimsAsync(targetRole);
                    var currentPerms = currentClaims.Where(c => c.Type == "Permission")
                                                    .Select(c => c.Value)
                                                    .ToHashSet();

                    // 2. Define the NEW permissions they MUST have
                    var newPermissions = new List<string>
                    {
                        AppPermissions.Pos.Access,           // The critical one
                        AppPermissions.Orders.View,
                        AppPermissions.Orders.Create,
                        AppPermissions.Orders.Edit,
                        AppPermissions.Orders.Delete,
                        AppPermissions.Inventory.View,
                        AppPermissions.Inventory.Edit
                    };

                    // 3. Assign missing ones
                    foreach (var perm in newPermissions)
                    {
                        if (!currentPerms.Contains(perm))
                        {
                            await roleManager.AddClaimAsync(targetRole, new Claim("Permission", perm));
                        }
                    }
                }
            }
        }
    }
}