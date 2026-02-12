using Microsoft.EntityFrameworkCore;
using PosSystem.Data;
using PosSystem.Data.Entities;

namespace PosSystem.Services
{
    public class SystemSettingsService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly ITenantService _tenantService; // [NEW] Inject TenantService

        public SystemSettingsService(IDbContextFactory<ApplicationDbContext> contextFactory, ITenantService tenantService)
        {
            _contextFactory = contextFactory;
            _tenantService = tenantService;
        }

        // --- [NEW] Methods called by SmsService & HrmService ---
        // These automatically use the current Tenant ID
        public async Task<string?> GetSettingAsync(string key)
        {
            var tenantId = _tenantService.GetTenantId();
            return await GetValueAsync(key, tenantId);
        }

        public async Task SetSettingAsync(string key, string value)
        {
            var tenantId = _tenantService.GetTenantId();
            await SetValueAsync(key, value, tenantId);
        }

        // --- EXISTING METHODS (Preserved for compatibility) ---
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
                context.SystemSettings.Add(new SystemSetting
                {
                    Id = Guid.NewGuid().ToString(), // Ensure ID is generated
                    Key = key,
                    Value = value,
                    TenantId = tenantId
                });
            }
            else
            {
                setting.Value = value;
            }
            await context.SaveChangesAsync();
        }
    }
}