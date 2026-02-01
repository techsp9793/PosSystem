using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PosSystem.Hubs;
using PosSystem.Services; // Add Services namespace
using Razorpay.Api;
using Newtonsoft.Json.Linq;

namespace PosSystem.Controllers
{
    [Route("api/razorpay")]
    [ApiController]
    public class RazorpayWebhookController : ControllerBase
    {
        private readonly IHubContext<PaymentHub> _hubContext;
        private readonly IPaymentService _paymentService; // [NEW] Inject PaymentService

        // Update Constructor
        public RazorpayWebhookController(IHubContext<PaymentHub> hubContext, IPaymentService paymentService)
        {
            _hubContext = hubContext;
            _paymentService = paymentService;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> ReceiveWebhook()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                string jsonBody = await reader.ReadToEndAsync();

                string signature = Request.Headers["X-Razorpay-Signature"];

                // [NEW] Get Secret from DB (Decrypted)
                string webhookSecret = await _paymentService.GetWebhookSecretAsync();

                if (string.IsNullOrEmpty(webhookSecret))
                {
                    // Fail safe if secret isn't set in DB
                    return StatusCode(500, "Webhook Secret not configured in System.");
                }

                // Verify using the DB secret
                Utils.verifyWebhookSignature(jsonBody, signature, webhookSecret);

                // ... (Rest of logic remains same) ...
                JObject data = JObject.Parse(jsonBody);
                string eventType = data["event"]?.ToString();

                if (eventType == "payment.captured")
                {
                    var payment = data["payload"]["payment"]["entity"];
                    decimal amount = (decimal)payment["amount"] / 100M;
                    string paymentId = payment["id"].ToString();
                    string payerContact = payment["contact"]?.ToString() ?? "";

                    await _hubContext.Clients.All.SendAsync("PaymentVerified", amount, paymentId, payerContact);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }
    }
}