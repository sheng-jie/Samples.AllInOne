namespace MassTransit.CourierDemo.Shared.Models;

public interface IOrderAmountRequest
{
    public string OrderId { get; }
}