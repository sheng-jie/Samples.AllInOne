namespace MassTransit.SmDemo.Share.Events;

public interface IOrderPaidEvent
{
    public string OrderId { get; set; }
}