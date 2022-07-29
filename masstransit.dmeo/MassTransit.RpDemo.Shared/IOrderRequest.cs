namespace MassTransit.RequestResponseDemo;

public interface IOrderRequest
{
    public string OrderId { get; }
}