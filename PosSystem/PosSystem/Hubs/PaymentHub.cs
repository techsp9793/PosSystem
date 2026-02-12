using Microsoft.AspNetCore.SignalR;

namespace PosSystem.Hubs
{
    public class PaymentHub : Hub
    {
        // 1. POS Client calls this when they open the "Pay" dialog
        public async Task JoinOrderGroup(string orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, orderId);
        }

        // 2. POS Client calls this when they close the dialog
        public async Task LeaveOrderGroup(string orderId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, orderId);
        }
    }
}