using System.Collections.Concurrent;
using MassTransit.SmDemo.OrderService.Domains;

namespace MassTransit.SmDemo.OrderService.Repositories;

public static class OrderRepository
{
    private static ConcurrentBag<Order> Orders { get; set; } = new ConcurrentBag<Order>();

    public static Task Insert(Order order)
    {
        Orders.Add(order);
        return Task.CompletedTask;
    }

    public static Task<Order> Get(Guid orderId)
    {
        var order = Orders.FirstOrDefault(o => o.OrderId == orderId);
        return Task.FromResult(order);
    }
    
}