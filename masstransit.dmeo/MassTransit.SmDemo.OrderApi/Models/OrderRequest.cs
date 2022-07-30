using MassTransit.SmDemo.Share.Contracts;

namespace MassTransit.SmDemo.OrderApi.Models;

public class OrderRequest : ISubmitOrderRequest
{
    public string UserId { get; set; }
    public List<OrderItem> OrderItems { get; set; }
}