using Microsoft.EntityFrameworkCore;
using PosSystem.Data;
using PosSystem.Helpers;
using Razorpay.Api;

namespace PosSystem.Services
{
    public interface IPaymentService
    {
        // Creates an order on Razorpay server and returns the "order_id" (e.g., "order_EKwx...")
        Task<string> CreateOrderAsync(decimal amount, string receiptId);

        // Verifies the signature returned by Razorpay after payment to ensure it's not fake
        Task<bool> VerifyPaymentSignature(string orderId, string paymentId, string signature);
        Task<string> GetPublicKeyAsync();
        Task<string> GetWebhookSecretAsync(); // Add this
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

        public async Task<string> CreateOrderAsync(decimal amount, string receiptId)
        {
            var keys = await GetKeysAsync();

            if (string.IsNullOrEmpty(keys.KeyId) || string.IsNullOrEmpty(keys.KeySecret))
            {
                throw new Exception("Payment Gateway is not configured for this store.");
            }

            try
            {
                // 1. Initialize Razorpay Client with Tenant's Keys
                var client = new RazorpayClient(keys.KeyId, keys.KeySecret);

                // 2. Prepare Options
                // Razorpay expects amount in PAISE (e.g., 100.00 INR -> 10000 paise)
                var options = new Dictionary<string, object>
                {
                    { "amount", (int)(amount * 100) },
                    { "currency", "INR" },
                    { "receipt", receiptId },
                    { "payment_capture", 1 } // Auto-capture payment
                };

                // 3. Create Order
                Order order = client.Order.Create(options);
                return order["id"].ToString();
            }
            catch (Exception ex)
            {
                // Log this error in real app
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

                // This utility throws an exception if verification fails
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

            // Look for the Key in SystemSettings
            var setting = await db.SystemSettings
                .FirstOrDefaultAsync(c => c.Key == "Razorpay_WebhookSecret");

            if (setting == null || string.IsNullOrEmpty(setting.Value)) return "";

            try
            {
                return EncryptionHelper.Decrypt(setting.Value);
            }
            catch
            {
                return "";
            }
        }
        // --- HELPER: Get Keys from DB ---
        private async Task<(string KeyId, string KeySecret)> GetKeysAsync()
        {
            var tenantId = _tenantService.GetTenantId();
            using var context = await _dbFactory.CreateDbContextAsync();

            var settings = await context.SystemSettings
                .Where(s => s.TenantId == tenantId && s.Key.StartsWith("Payment_Razorpay"))
                .ToListAsync();

            var keyId = settings.FirstOrDefault(s => s.Key == "Payment_RazorpayKeyId")?.Value ?? "";
            var keySecret = settings.FirstOrDefault(s => s.Key == "Payment_RazorpayKeySecret")?.Value ?? "";

            return (keyId, keySecret);
        }
    }
}