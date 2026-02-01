using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PosSystem.Data.Entities;
using System.Security.Claims;
using PosSystem.Permissions;
using PosSystem.Helpers; // [NEW] Required for EncryptionHelper

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

            // =========================================================
            // --- [NEW] STEP 1: SEED SECURE CONFIGURATIONS ---
            // =========================================================
            // This ensures the Webhook Secret exists in the DB (Encrypted)
            if (!context.SystemSettings.Any(c => c.Key == "Razorpay_WebhookSecret"))
            {
                context.SystemSettings.Add(new SystemSetting
                {
                    Key = "Razorpay_WebhookSecret",
                    // We encrypt the default local secret. 
                    // You can change "my_local_secret_123" to match your appsettings if needed.
                    Value = EncryptionHelper.Encrypt("my_local_secret_123")
                    
                });
                // Save immediately so it's available for the rest of the app
                await context.SaveChangesAsync();
            }

            // Read SuperAdmin Config
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
            // --- STEP 6: SYNC TENANT ADMIN / MANAGER ROLE ---
            // =========================================================
            // This grants POS access to your existing "Admin" users automatically.

            string tenantRoleName = "Admin";

            if (await roleManager.RoleExistsAsync(tenantRoleName))
            {
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
                        AppPermissions.Pos.Access,
                        AppPermissions.Orders.View,
                        AppPermissions.Orders.Create,
                        AppPermissions.Orders.Edit,
                        AppPermissions.Orders.Delete,
                        AppPermissions.Inventory.View,
                        AppPermissions.Inventory.Edit,
                        AppPermissions.Settings.Receipts,
                        AppPermissions.Settings.Payments ,
                        AppPermissions.Facilities.View,
                        AppPermissions.Facilities.GenerateQr,
                        AppPermissions.Facilities.Gatekeeper
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