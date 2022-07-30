using MassTransit.SmDemo.Share.Contracts;

namespace MassTransit.SmDemo.OrderApi.Models;

public class PayRequest :IPayOrderRequest
{
    public string UserId { get; set; }
    public string OrderId { get; set; }
    public double Amount { get; set; }
}