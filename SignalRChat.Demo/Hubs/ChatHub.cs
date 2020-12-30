using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Demo.Hubs
{
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {

            //Groups.AddToGroupAsync(Context.ConnectionId, "lua-product-id");
            await Clients.Others.SendAsync("ReceiveMessage", "Connected", Context.ConnectionId, Context.User.Identity.Name);

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "Connected", Context.ConnectionId, "lua");
            await Clients.All.SendAsync("ReceiveMessage", user, message, Context.ConnectionId);
        }


    }
}