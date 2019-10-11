using System.Threading.Tasks;

namespace SignalRChat.Demo.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(string user, string message, string connectionId);
        Task ReceiveMessage(string message, string connectionId);
    }
}