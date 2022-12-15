using System.Collections.Concurrent;
using MassTransit.CourierDemo.OrderService.Domains;

namespace MassTransit.CourierDemo.OrderService.Repositories;

public static class OrderRepository
{
    private static ConcurrentBag<Order> Orders { get; set; } = new ConcurrentBag<Order>();

    public static Task Insert(Order order)
    {
        Orders.Add(order);
        return Task.CompletedTask;
    }

    public static Task<Order> Get(string orderId)
    {
        var order = Orders.FirstOrDefault(o => o.OrderId == orderId);
        return Task.FromResult(order);
    }
    
}