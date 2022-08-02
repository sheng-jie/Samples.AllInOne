namespace MassTransit.SmDemo.Share.Events;

public interface IOrderCanceledEvent
{
    public Guid OrderId { get; set; }
}