using Orleans.Streaming.GrainInterfaces;
using Orleans.Streams;

namespace Orleans.Streaming.Grains;

public class ChatRoomGrain : Grain, IChatRoomGrain
{
    private readonly List<ChatMsg> _chatMsgs = new List<ChatMsg>(200);
    private readonly List<string> _onlineMembers = new List<string>(10);
    private IAsyncStream<ChatMsg> _stream = null!;

    public override Task OnActivateAsync()
    {
        var streamProvider = GetStreamProvider("chat");

        _stream = streamProvider.GetStream<ChatMsg>(
            Guid.NewGuid(), "default");

        return base.OnActivateAsync();
    }

    public async Task<Guid> Join(string nickname)
    {
        _onlineMembers.Add(nickname);
        await _stream.OnNextAsync(new ChatMsg("System", $"{nickname} join the chat room!"));

        return _stream.Guid;
    }

    public async Task<Guid> Leave(string nickname)
    {
        _onlineMembers.Remove(nickname);
        await _stream.OnNextAsync(new ChatMsg("System", $"{nickname} leave the chat room!"));

        return _stream.Guid;
    }

    public async Task<bool> Send(ChatMsg msg)
    {
        _chatMsgs.Add(msg);

        await _stream.OnNextAsync(msg);

        return true;
    }
}