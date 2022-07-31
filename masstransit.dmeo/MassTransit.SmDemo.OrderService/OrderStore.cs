using MassTransit.SmDemo.Share.Contracts;

namespace MassTransit.SmDemo.OrderService;

public static class OrderStore
{
    public static List<Order> Orders = new List<Order>();

    public static Order GetOrder(string orderId)
    {
        return Orders.Find(t => t.OrderId == orderId);
    }

    public static void AddOrder(Order order)
    {
        Orders.Add(order);
    }

    public static void UpdateOrderState(string orderId, OrderStatus newStatus)
    {
        var order = GetOrder(orderId);
        order.Status = newStatus;
        if (newStatus == OrderStatus.Paid)
        {
            order.PayTime = DateTime.Now;
        }
    }
}