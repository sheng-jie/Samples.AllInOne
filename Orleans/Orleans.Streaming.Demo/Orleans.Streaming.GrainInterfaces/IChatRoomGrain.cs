namespace Orleans.Streaming.GrainInterfaces;

public interface IChatRoomGrain : IGrainWithGuidKey
{
    Task<Guid> Join(string nickname);
    Task<Guid> Leave(string nickname);
    Task<bool> Send(ChatMsg msg);
}

[Serializable]
public record ChatMsg(string Member, string Text)
{
    public DateTimeOffset Created { get; init; } = DateTimeOffset.Now;
}