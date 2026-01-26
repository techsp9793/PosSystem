using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace PosSystem.Services
{
    // FIX: Add SetTenantId here
    public interface ITenantService
    {
        string? GetTenantId();
        string? GetUserId();
        void SetTenantId(string tenantId);
    }

    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TenantService> _logger;
        private string? _manualTenantId;

        public TenantService(IHttpContextAccessor httpContextAccessor, ILogger<TenantService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public string? GetTenantId()
        {
            // Priority: Manual Set (for Blazor Components)
            if (!string.IsNullOrEmpty(_manualTenantId))
                return _manualTenantId;

            try
            {
                // Fallback: HttpContext (for Controllers / API)
                var user = _httpContextAccessor.HttpContext?.User;
                var tenantId = user?.FindFirst("tenant_id")?.Value;
                return tenantId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving TenantId.");
                return null;
            }
        }

        public string? GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        // Implementation of the new method
        public void SetTenantId(string tenantId)
        {
            _manualTenantId = tenantId;
        }
    }
}