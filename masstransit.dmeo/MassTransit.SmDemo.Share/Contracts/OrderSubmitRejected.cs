namespace MassTransit.SmDemo.Share.Contracts;

public interface OrderSubmitRejected
{
    public string Reason { get; set; }
}