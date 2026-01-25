using Microsoft.JSInterop;
using MudBlazor;

namespace PosSystem.Client.Services
{
    public class ThemeService
    {
        private readonly IJSRuntime _js;
        private const string StorageKey = "isDarkMode";

        public bool IsDarkMode { get; private set; }
        public event Action? OnChange;

        public ThemeService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task InitializeAsync()
        {
            try
            {
                var stored = await _js.InvokeAsync<string>("localStorage.getItem", StorageKey);
                if (bool.TryParse(stored, out var isDark) && isDark != IsDarkMode)
                {
                    IsDarkMode = isDark;
                    await UpdateHtmlClass();
                    OnChange?.Invoke();
                }
            }
            catch { /* Ignore during pre-render */ }
        }

        public async Task SetDarkModeAsync(bool value)
        {
            if (IsDarkMode == value) return;

            IsDarkMode = value;
            await UpdateHtmlClass();

            try
            {
                await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, value.ToString().ToLower());
                var cookieScript = $"document.cookie = 'isDarkMode={value.ToString().ToLower()}; path=/; max-age=31536000; SameSite=Lax'";
                await _js.InvokeVoidAsync("eval", cookieScript);
            }
            catch { /* Silent fail during pre-rendering */ }

            OnChange?.Invoke();
        }

        // Synchronous version for pre-render compatibility
        public void SetDarkMode(bool value)
        {
            if (IsDarkMode == value) return;
            IsDarkMode = value;
            OnChange?.Invoke();

            _ = Task.Run(async () =>
            {
                try
                {
                    await UpdateHtmlClass();
                    await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, value.ToString().ToLower());
                    var cookieScript = $"document.cookie = 'isDarkMode={value.ToString().ToLower()}; path=/; max-age=31536000; SameSite=Lax'";
                    await _js.InvokeVoidAsync("eval", cookieScript);
                }
                catch { /* Silent fail */ }
            });
        }
        public void ShowMessage(string message, Severity severity = Severity.Info)
        {
            // We'll inject ISnackbar in pages and use this as a helper later
            // For now, leave empty or add event if you want centralized toast
        }
        private async Task UpdateHtmlClass()
        {
            try
            {
                await _js.InvokeVoidAsync("updateThemeClass", IsDarkMode);
            }
            catch { /* Ignore if JS not ready */ }
        }
    }
}