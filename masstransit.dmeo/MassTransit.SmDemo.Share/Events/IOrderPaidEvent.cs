namespace MassTransit.SmDemo.Share.Events;

public interface IOrderPaidEvent
{
    public Guid OrderId { get; set; }
}