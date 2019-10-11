using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Demo.Hubs
{
    public class StronglyTypedChatHub : Hub<IChatClient>
    {
        public override async Task OnConnectedAsync()
        {

            //Groups.AddToGroupAsync(Context.ConnectionId, "lua-product-id");
            await Clients.Others.ReceiveMessage("Connected", Context.ConnectionId, Context.User.Identity.Name);

            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.ReceiveMessage(user, message, Context.ConnectionId);
        }

        public Task SendMessageToCaller(string message)
        {
            return Clients.Caller.ReceiveMessage(message, Context.ConnectionId);
        }
    }
}