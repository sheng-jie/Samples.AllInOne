namespace MassTransit.CourierDemo.Shared.Models;

public interface IOrderAmountResponse
{
    public string OrderId { get; }
    public decimal Amount { get; set; }
}