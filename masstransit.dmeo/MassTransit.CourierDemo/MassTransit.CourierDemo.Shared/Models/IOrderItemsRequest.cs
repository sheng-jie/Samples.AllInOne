namespace MassTransit.CourierDemo.Shared.Models;

public interface IOrderItemsRequest
{
    public string OrderId { get; }
}