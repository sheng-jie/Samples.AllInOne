namespace MassTransit.SmDemo.Share.Events;

public interface IOrderShippedEvent
{
    public Guid OrderId { get; set; }
}