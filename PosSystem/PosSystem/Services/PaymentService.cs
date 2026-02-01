using Microsoft.EntityFrameworkCore;
using PosSystem.Data;
using PosSystem.Data.Entities;
using PosSystem.Helpers;
using Razorpay.Api;

namespace PosSystem.Services
{
    public interface IPaymentService
    {
        Task<string> CreateOrderAsync(decimal amount, string receiptId, Dictionary<string, string>? notes = null);
        Task<bool> VerifyPaymentSignature(string orderId, string paymentId, string signature);
        Task<string> GetPublicKeyAsync();
        Task<string> GetWebhookSecretAsync();
    }

    public class PaymentService : IPaymentService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly ITenantService _tenantService;
        private readonly IServiceScopeFactory _scopeFactory;

        public PaymentService(IDbContextFactory<ApplicationDbContext> dbFactory, ITenantService tenantService, IServiceScopeFactory scopeFactory)
        {
            _dbFactory = dbFactory;
            _tenantService = tenantService;
            _scopeFactory = scopeFactory;
        }

        public async Task<string> CreateOrderAsync(decimal amount, string receiptId, Dictionary<string, string>? notes = null)
        {
            var keys = await GetKeysAsync();

            if (string.IsNullOrEmpty(keys.KeyId) || string.IsNullOrEmpty(keys.KeySecret))
            {
                throw new Exception("Payment Gateway is not configured for this store.");
            }

            try
            {
                var client = new RazorpayClient(keys.KeyId, keys.KeySecret);

                var options = new Dictionary<string, object>
                {
                    { "amount", (int)(amount * 100) },
                    { "currency", "INR" },
                    { "receipt", receiptId },
                    { "payment_capture", 1 }
                };

                if (notes != null)
                {
                    options.Add("notes", notes);
                }

                // [FIX: Ambiguity Solved]
                // We specify 'Razorpay.Api.Order' so it doesn't confuse it with your Database Order entity.
                Razorpay.Api.Order order = client.Order.Create(options);

                return order["id"].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Razorpay Error: {ex.Message}");
            }
        }

        public async Task<bool> VerifyPaymentSignature(string orderId, string paymentId, string signature)
        {
            var keys = await GetKeysAsync();

            try
            {
                var client = new RazorpayClient(keys.KeyId, keys.KeySecret);

                var attributes = new Dictionary<string, string>
                {
                    { "razorpay_order_id", orderId },
                    { "razorpay_payment_id", paymentId },
                    { "razorpay_signature", signature }
                };

                // Using the 1-argument version as per your working code
                Utils.verifyPaymentSignature(attributes);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetPublicKeyAsync()
        {
            var keys = await GetKeysAsync();
            return keys.KeyId;
        }

        public async Task<string> GetWebhookSecretAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var setting = await db.SystemSettings
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Key == "Razorpay_WebhookSecret");

            if (setting == null || string.IsNullOrEmpty(setting.Value)) return "";

            try
            {
                return EncryptionHelper.Decrypt(setting.Value);
            }
            catch
            {
                return setting.Value;
            }
        }

        private async Task<(string KeyId, string KeySecret)> GetKeysAsync()
        {
            var tenantId = _tenantService.GetTenantId();
            using var context = await _dbFactory.CreateDbContextAsync();

            IQueryable<SystemSetting> query = context.SystemSettings;

            if (!string.IsNullOrEmpty(tenantId))
            {
                query = query.Where(s => s.TenantId == tenantId);
            }
            else
            {
                query = query.IgnoreQueryFilters();
            }

            var settings = await query
                .Where(s => s.Key.StartsWith("Payment_Razorpay"))
                .ToListAsync();

            var keyId = settings.FirstOrDefault(s => s.Key == "Payment_RazorpayKeyId")?.Value ?? "";
            var secretRaw = settings.FirstOrDefault(s => s.Key == "Payment_RazorpayKeySecret")?.Value ?? "";

            var keySecret = secretRaw;

            if (!string.IsNullOrEmpty(secretRaw))
            {
                try
                {
                    keySecret = EncryptionHelper.Decrypt(secretRaw);
                }
                catch
                {
                    keySecret = secretRaw;
                }
            }

            return (keyId, keySecret);
        }
    }
}