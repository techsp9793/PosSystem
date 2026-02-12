using PosSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace PosSystem.Services
{
    public class SmsService
    {
        private readonly SystemSettingsService _settings;
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public SmsService(SystemSettingsService settings, IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _settings = settings;
            _dbFactory = dbFactory;
        }

        public async Task SaveConfigAsync(string provider, string url, string apiKey, string senderId)
        {
            await _settings.SetSettingAsync("SMS_Provider", provider); // e.g., "Twilio", "TextLocal", "Fast2SMS"
            await _settings.SetSettingAsync("SMS_Url", url);
            await _settings.SetSettingAsync("SMS_ApiKey", apiKey);
            await _settings.SetSettingAsync("SMS_SenderId", senderId);
        }

        public async Task<(string Provider, string Url, string Key, string Sender)> GetConfigAsync()
        {
            return (
                await _settings.GetSettingAsync("SMS_Provider") ?? "Generic",
                await _settings.GetSettingAsync("SMS_Url") ?? "",
                await _settings.GetSettingAsync("SMS_ApiKey") ?? "",
                await _settings.GetSettingAsync("SMS_SenderId") ?? ""
            );
        }

        public async Task<bool> SendPaymentLinkAsync(string mobile, decimal amount, string paymentUrl)
        {
            var config = await GetConfigAsync();
            if (string.IsNullOrEmpty(config.Url)) return false; // Not configured

            string message = $"Dear Customer, please pay {amount:C} using this link: {paymentUrl}";

            try
            {
                // TODO: IMPLEMENT ACTUAL API CALL HERE using HttpClient
                // For now, we simulate success so you can test the UI flow.
                Console.WriteLine($"[SMS MOCK] To: {mobile} | Msg: {message}");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}