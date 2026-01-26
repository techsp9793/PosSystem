using PosSystem.Permissions;
using PosSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace PosSystem.Data.Seeders
{
    public static class PermissionSeeder
    {
        public static async Task SeedPermissionsAsync(IServiceProvider serviceProvider)
        {
            // Removed 'using var scope' because we are using the provider passed from Program.cs
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Get Code Permissions
            var codePermissions = AppPermissions.GetAllPermissions();

            // Get DB Permissions
            var dbPermissions = await context.SystemPermissions.ToListAsync();
            var dbCodes = dbPermissions.Select(p => p.PermissionCode).ToHashSet();

            // Insert New Ones
            var newPermissions = new List<SystemPermission>();
            foreach (var code in codePermissions)
            {
                if (!dbCodes.Contains(code))
                {
                    var parts = code.Split('.');
                    var group = parts.Length > 2 ? parts[1] : "General";
                    var name = parts.Last();

                    newPermissions.Add(new SystemPermission
                    {
                        PermissionCode = code,
                        GroupName = group,
                        Name = name
                    });
                }
            }

            if (newPermissions.Any())
            {
                await context.SystemPermissions.AddRangeAsync(newPermissions);
                await context.SaveChangesAsync();
            }
        }
    }
}