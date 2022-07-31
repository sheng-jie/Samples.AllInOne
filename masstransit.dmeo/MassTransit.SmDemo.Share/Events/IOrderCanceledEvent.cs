namespace MassTransit.SmDemo.Share.Events;

public interface IOrderCanceledEvent
{
    public string OrderId { get; set; }
}