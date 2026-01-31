using Microsoft.EntityFrameworkCore;
using PosSystem.Data;

namespace PosSystem.Services
{
    public class TerminologyService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private Dictionary<string, string> _terms = new();
        public bool IsLoaded { get; private set; } = false;

        public TerminologyService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        // Call this when the user logs in or the app starts (e.g. in MainLayout)
        public async Task LoadTermsAsync(string tenantId)
        {
            if (string.IsNullOrEmpty(tenantId)) return;

            try
            {
                using var context = await _dbFactory.CreateDbContextAsync();

                // Fetch all custom terms for this specific tenant
                var termsList = await context.TenantTerminologies
                    .Where(t => t.TenantId == tenantId)
                    .AsNoTracking()
                    .ToListAsync();

                // Convert to Dictionary for fast lookup
                _terms = termsList.ToDictionary(k => k.TermKey, v => v.TermValue);

                IsLoaded = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading terminology: {ex.Message}");
                // Fallback: Dictionary remains empty, system uses default strings
            }
        }

        // The main method used in Razor pages
        // Example: Terms.GetText("Label_Product", "Product")
        public string GetText(string key, string defaultText = "")
        {
            if (_terms.TryGetValue(key, out var value))
            {
                return value;
            }

            // If key not found, return the default text provided by the UI, 
            // or the key itself if no default is provided.
            return string.IsNullOrEmpty(defaultText) ? key : defaultText;
        }

        // Helper to refresh if settings change
        public void ClearCache()
        {
            _terms.Clear();
            IsLoaded = false;
        }
    }
}