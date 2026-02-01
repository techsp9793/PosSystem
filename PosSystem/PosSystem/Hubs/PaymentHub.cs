using Microsoft.AspNetCore.SignalR;

namespace PosSystem.Hubs
{
    public class PaymentHub : Hub
    {
        // We will call this from the Controller, so we don't need methods here yet.
        // The POS will just "Listen" to this Hub.
    }
}