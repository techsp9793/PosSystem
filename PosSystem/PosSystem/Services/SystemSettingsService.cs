using Microsoft.EntityFrameworkCore;
using PosSystem.Data;
using PosSystem.Data.Entities;

namespace PosSystem.Services
{
    public class SystemSettingsService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public SystemSettingsService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<string> GetValueAsync(string key, string? tenantId = null)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var setting = await context.SystemSettings
                .FirstOrDefaultAsync(s => s.Key == key && s.TenantId == tenantId);

            return setting?.Value ?? string.Empty;
        }

        public async Task SetValueAsync(string key, string value, string? tenantId = null)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var setting = await context.SystemSettings
                .FirstOrDefaultAsync(s => s.Key == key && s.TenantId == tenantId);

            if (setting == null)
            {
                context.SystemSettings.Add(new SystemSetting { Key = key, Value = value, TenantId = tenantId });
            }
            else
            {
                setting.Value = value;
            }
            await context.SaveChangesAsync();
        }
    }
}