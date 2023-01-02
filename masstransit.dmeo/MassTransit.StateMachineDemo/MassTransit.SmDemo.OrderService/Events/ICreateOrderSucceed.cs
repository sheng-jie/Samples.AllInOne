using MassTransit.SmDemo.OrderService.Domains;

namespace MassTransit.SmDemo.OrderService.Events;

public interface ICreateOrderSucceed
{
    public Guid OrderId { get; set; }

    public List<OrderItem> OrderItems { get; set; }
}