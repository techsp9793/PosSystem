using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PosSystem.Data;
using PosSystem.Hubs;
using PosSystem.Services;
using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;

namespace PosSystem.Controllers
{
    [Route("api/razorpay")]
    [ApiController]
    public class RazorpayWebhookController : ControllerBase
    {
        private readonly IHubContext<PaymentHub> _hubContext;
        private readonly IPaymentService _paymentService;
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly ILogger<RazorpayWebhookController> _logger;

        public RazorpayWebhookController(
            IHubContext<PaymentHub> hubContext,
            IPaymentService paymentService,
            IDbContextFactory<ApplicationDbContext> dbFactory,
            ILogger<RazorpayWebhookController> logger)
        {
            _hubContext = hubContext;
            _paymentService = paymentService;
            _dbFactory = dbFactory;
            _logger = logger;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> ReceiveWebhook()
        {
            string jsonBody = "";
            try
            {
                // 1. Read Request Body
                using var reader = new StreamReader(Request.Body);
                jsonBody = await reader.ReadToEndAsync();

                string signature = Request.Headers["X-Razorpay-Signature"].ToString();

                // 2. Get Secret & Verify
                string webhookSecret = await _paymentService.GetWebhookSecretAsync();
                if (string.IsNullOrEmpty(webhookSecret))
                {
                    _logger.LogError("Webhook Secret is missing in DB settings.");
                    return StatusCode(500, "Webhook Secret not configured.");
                }

                // Verify Signature (Manual implementation avoids dependency issues)
                if (!VerifySignature(jsonBody, signature, webhookSecret))
                {
                    _logger.LogWarning("Invalid Webhook Signature.");
                    return BadRequest("Invalid Signature");
                }

                // 3. Parse Event
                JObject data = JObject.Parse(jsonBody);
                string eventType = data["event"]?.ToString();

                // We listen for 'payment.captured' or 'order.paid'
                if (eventType == "payment.captured" || eventType == "order.paid")
                {
                    var payload = data["payload"];
                    var paymentEntity = payload?["payment"]?["entity"];
                    var orderEntity = payload?["order"]?["entity"];

                    // Get the Razorpay Order ID (e.g., "order_DaZ3...")
                    // Note: 'order.paid' has it in order->entity->id
                    //       'payment.captured' has it in payment->entity->order_id
                    string rzOrderId = orderEntity?["id"]?.ToString()
                                       ?? paymentEntity?["order_id"]?.ToString();

                    if (!string.IsNullOrEmpty(rzOrderId))
                    {
                        await ProcessSuccessfulPayment(rzOrderId, paymentEntity);
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Razorpay Webhook");
                return BadRequest("Error processing webhook");
            }
        }

        private async Task ProcessSuccessfulPayment(string rzOrderId, JToken? paymentData)
        {
            using var db = await _dbFactory.CreateDbContextAsync();

            // 1. Find the internal Order using the Razorpay Reference ID
            var order = await db.Orders.FirstOrDefaultAsync(o => o.PaymentReferenceId == rzOrderId);

            if (order != null)
            {
                // 2. Update Database Status
                // Only update if it isn't already completed (prevents duplicate logic)
                if (order.Status != "Completed")
                {
                    order.Status = "Completed";
                    order.PaymentMethod = "Online"; // Or specific method like "UPI"
                    order.IsSynced = true;
                    order.SyncedAt = DateTime.UtcNow;

                    // Optional: Save contact info if provided by webhook
                    if (paymentData != null)
                    {
                        var email = paymentData["email"]?.ToString();
                        var contact = paymentData["contact"]?.ToString();
                        // You could save these to Customer fields if needed
                    }

                    await db.SaveChangesAsync();
                    _logger.LogInformation($"Order {order.OrderNumber} marked as PAID.");

                    // 3. SIGNALR: Notify ONLY the specific POS screen
                    // We send a message to the Group named after the Internal Order ID
                    await _hubContext.Clients.Group(order.Id).SendAsync("ReceivePaymentConfirmation", "PAID");
                }
            }
            else
            {
                _logger.LogWarning($"Webhook received for unknown OrderID: {rzOrderId}");
            }
        }

        private bool VerifySignature(string payload, string signature, string secret)
        {
            if (string.IsNullOrEmpty(signature)) return false;

            using var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(secret));
            var hashBytes = hmac.ComputeHash(Encoding.ASCII.GetBytes(payload));
            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString == signature;
        }
    }
}