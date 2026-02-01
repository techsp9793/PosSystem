using PosSystem.Data.Entities;
using Microsoft.JSInterop;
using Microsoft.EntityFrameworkCore;
using PosSystem.Data;

namespace PosSystem.Services
{
    public class ReceiptService
    {
        private readonly IJSRuntime _js;
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        // We inject DbFactory to fetch settings on-the-fly
        public ReceiptService(IJSRuntime js, IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _js = js;
            _dbFactory = dbFactory;
        }

        public async Task PrintAsync(Order order, string defaultStoreName, string defaultAddress)
        {
            // 1. Fetch Tenant Settings
            var config = await GetReceiptConfigAsync(order.TenantId, defaultStoreName);

            // 2. Generate Smart HTML
            var html = GenerateHtml(order, config, defaultAddress);

            // 3. Send to JS for Printing
            await _js.InvokeVoidAsync("printReceipt", html);
        }

        private async Task<ReceiptConfig> GetReceiptConfigAsync(string tenantId, string defaultName)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var settings = await context.SystemSettings
                .Where(s => s.TenantId == tenantId && s.Key.StartsWith("Receipt_"))
                .ToListAsync();

            return new ReceiptConfig
            {
                Header = settings.FirstOrDefault(s => s.Key == "Receipt_Header")?.Value ?? defaultName,
                LogoBase64 = settings.FirstOrDefault(s => s.Key == "Receipt_LogoBase64")?.Value ?? "",
                PaperSize = settings.FirstOrDefault(s => s.Key == "Receipt_PaperSize")?.Value ?? "80mm",
                FooterMessage = settings.FirstOrDefault(s => s.Key == "Receipt_FooterMessage")?.Value ?? "Thank you!",
                ShowQrCode = bool.Parse(settings.FirstOrDefault(s => s.Key == "Receipt_ShowQrCode")?.Value ?? "true")
            };
        }

        private string GenerateHtml(Order order, ReceiptConfig config, string address)
        {
            // Calculate Width: 
            // 80mm paper has ~72mm print width (~270px at 96dpi, but safely 300px)
            // 58mm paper has ~48mm print width (~180px at 96dpi, but safely 200px)
            string widthCss = config.PaperSize == "58mm" ? "190px" : "290px";
            string fontSize = config.PaperSize == "58mm" ? "10px" : "12px";

            var sb = new System.Text.StringBuilder();
            sb.Append($@"
                <html>
                <head>
                    <style>
                        body {{ font-family: 'Courier New', monospace; width: {widthCss}; margin: 0; padding: 2px; font-size: {fontSize}; color: black; background: white; }}
                        .text-center {{ text-align: center; }}
                        .text-right {{ text-align: right; }}
                        .bold {{ font-weight: bold; }}
                        .line {{ border-bottom: 1px dashed #000; margin: 5px 0; }}
                        table {{ width: 100%; border-collapse: collapse; }}
                        td {{ vertical-align: top; padding: 2px 0; }}
                        img.logo {{ max-width: 80%; max-height: 100px; display: block; margin: 0 auto 5px auto; }}
                        img.qr {{ width: 100px; height: 100px; margin: 5px auto; display: block; }}
                    </style>
                </head>
                <body>");

            // --- 1. LOGO ---
            if (!string.IsNullOrEmpty(config.LogoBase64))
            {
                sb.Append($"<img class='logo' src='{config.LogoBase64}' />");
            }
            else
            {
                // Fallback Text Header if no logo
                sb.Append($"<div class='text-center bold' style='font-size:1.2em'>{config.Header.ToUpper()}</div>");
            }

            // --- 2. HEADER INFO ---
            // If logo exists, we usually print header below it too, or just address. 
            // Let's print header text anyway as standard practice.
            if (!string.IsNullOrEmpty(config.LogoBase64))
            {
                sb.Append($"<div class='text-center bold'>{config.Header}</div>");
            }
            sb.Append($"<div class='text-center'>{address}</div>");
            sb.Append("<div class='line'></div>");

            // --- 3. ORDER METADATA ---
            sb.Append($"<div>Order: {order.OrderNumber}</div>");
            sb.Append($"<div>Date: {order.CreatedAt.ToLocalTime():dd/MM/yyyy HH:mm}</div>");
            if (!string.IsNullOrEmpty(order.CustomerName))
            {
                sb.Append($"<div>Cust: {order.CustomerName}</div>");
            }
            sb.Append("<div class='line'></div>");

            // --- 4. ITEMS ---
            sb.Append("<table>");
            foreach (var item in order.OrderItems)
            {
                // 58mm layout needs to be tighter
                sb.Append("<tr>");
                sb.Append($"<td colspan='2' class='bold'>{item.ProductName}</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append($"<td>{item.Quantity} x {item.UnitPrice:0.00}</td>");
                sb.Append($"<td class='text-right'>{item.TotalPrice:0.00}</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            sb.Append("<div class='line'></div>");

            // --- 5. TOTALS ---
            sb.Append("<table>");
            sb.Append($"<tr class='bold' style='font-size:1.1em'><td>TOTAL</td><td class='text-right'>{order.TotalAmount:0.00}</td></tr>");
            sb.Append($"<tr><td>Paid via</td><td class='text-right'>{order.PaymentMethod}</td></tr>");
            sb.Append("</table>");

            // --- 6. FOOTER & QR ---
            sb.Append("<div class='line'></div>");

            if (config.ShowQrCode)
            {
                // Simple QR generation for order verification (points to order URL or just Order ID)
                // We use a public API for now to generate the image
                string qrData = $"{config.Header}|{order.OrderNumber}|{order.TotalAmount}";
                sb.Append($"<img class='qr' src='https://api.qrserver.com/v1/create-qr-code/?size=100x100&data={System.Net.WebUtility.UrlEncode(qrData)}' />");
            }

            if (!string.IsNullOrEmpty(config.FooterMessage))
            {
                sb.Append($"<div class='text-center' style='margin-top:5px;'>{config.FooterMessage}</div>");
            }

            sb.Append("<div class='text-center' style='font-size:0.8em; margin-top:5px;'>*** THANK YOU ***</div>");

            sb.Append("</body></html>");
            return sb.ToString();
        }

        private class ReceiptConfig
        {
            public string Header { get; set; } = "";
            public string LogoBase64 { get; set; } = "";
            public string PaperSize { get; set; } = "80mm";
            public string FooterMessage { get; set; } = "";
            public bool ShowQrCode { get; set; } = true;
        }
    }
}