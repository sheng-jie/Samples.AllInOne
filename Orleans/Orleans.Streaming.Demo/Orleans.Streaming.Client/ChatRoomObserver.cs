using Orleans.Streaming.GrainInterfaces;
using Orleans.Streams;

namespace Orleans.Streaming.Client;

public class ChatRoomObserver:IAsyncObserver<ChatMsg>
{
    public Task OnNextAsync(ChatMsg item, StreamSequenceToken token = null)
    {
        Console.WriteLine($"[{item.Member}]({item.Created}):{item.Text}");
        return Task.CompletedTask;
    }

    public Task OnCompletedAsync()
    {
        throw new NotImplementedException();
    }

    public Task OnErrorAsync(Exception ex)
    {
        throw new NotImplementedException();
    }
}