using PosSystem.Data.Entities;
using Microsoft.JSInterop;

namespace PosSystem.Services
{
    public class ReceiptService
    {
        private readonly IJSRuntime _js;

        public ReceiptService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task PrintAsync(Order order, string storeName, string address)
        {
            var html = GenerateHtml(order, storeName, address);
            await _js.InvokeVoidAsync("printReceipt", html);
        }

        private string GenerateHtml(Order order, string storeName, string address)
        {
            // Simple CSS for 80mm thermal paper
            var sb = new System.Text.StringBuilder();
            sb.Append(@"
                <html>
                <head>
                    <style>
                        body { font-family: 'Courier New', monospace; width: 300px; margin: 0; padding: 10px; font-size: 12px; }
                        .text-center { text-align: center; }
                        .text-right { text-align: right; }
                        .bold { font-weight: bold; }
                        .line { border-bottom: 1px dashed #000; margin: 5px 0; }
                        table { width: 100%; border-collapse: collapse; }
                        td { vertical-align: top; }
                    </style>
                </head>
                <body>");

            // Header
            sb.Append($"<div class='text-center bold' style='font-size:16px'>{storeName}</div>");
            sb.Append($"<div class='text-center'>{address}</div>");
            sb.Append("<div class='line'></div>");

            // Order Info
            sb.Append($"<div>Order: {order.OrderNumber}</div>");
            sb.Append($"<div>Date: {order.CreatedAt.ToLocalTime():dd/MM/yyyy HH:mm}</div>");
            if (!string.IsNullOrEmpty(order.CustomerName))
            {
                sb.Append($"<div>Cust: {order.CustomerName}</div>");
            }
            sb.Append("<div class='line'></div>");

            // Items
            sb.Append("<table>");
            foreach (var item in order.OrderItems)
            {
                sb.Append("<tr>");
                sb.Append($"<td colspan='3'>{item.ProductName}</td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append($"<td>{item.Quantity} x {item.UnitPrice:0.00}</td>");
                sb.Append($"<td class='text-right'>{item.TotalPrice:0.00}</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            sb.Append("<div class='line'></div>");

            // Totals
            sb.Append("<table>");
            sb.Append($"<tr class='bold' style='font-size:14px'><td>TOTAL</td><td class='text-right'>{order.TotalAmount:0.00}</td></tr>");
            sb.Append($"<tr><td>Paid via</td><td class='text-right'>{order.PaymentMethod}</td></tr>");
            sb.Append("</table>");

            // Footer
            sb.Append("<div class='line'></div>");
            sb.Append("<div class='text-center'>Thank you for visiting!</div>");
            sb.Append("<div class='text-center'>*** Customer Copy ***</div>");

            sb.Append("</body></html>");
            return sb.ToString();
        }
    }
}