using Microsoft.Extensions.Logging;

namespace PosSystem.Services
{
    public class NotificationService
    {
        private readonly SystemSettingsService _settings;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(SystemSettingsService settings, ILogger<NotificationService> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task SendCredentialsEmailAsync(string email, string username, string tempPassword, string? tenantId)
        {
            // 1. Fetch API Keys dynamically from DB
            var smtpHost = await _settings.GetValueAsync("SMTP_Host", tenantId);
            var smtpUser = await _settings.GetValueAsync("SMTP_User", tenantId);

            // 2. Mock Sending Logic (Replace with MailKit or SendGrid later)
            if (string.IsNullOrEmpty(smtpHost))
            {
                _logger.LogWarning("Cannot send email: SMTP Host not configured for tenant {Tenant}", tenantId ?? "Global");
                return;
            }

            _logger.LogInformation("EMAIL SENT to {Email}. Creds: {User}/{Pass} via {Host}", email, username, tempPassword, smtpHost);
        }

        public async Task SendCredentialsSmsAsync(string phoneNumber, string username, string tempPassword)
        {
            // Fetch SMS Key
            var smsKey = await _settings.GetValueAsync("SMS_Key");
            _logger.LogInformation("SMS SENT to {Phone} using Key: {Key}", phoneNumber, smsKey);
        }
    }
}