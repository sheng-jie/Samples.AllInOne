namespace MassTransit.SmDemo.Share.Events;

public interface IOrderShippedEvent
{
    public string OrderId { get; set; }
}