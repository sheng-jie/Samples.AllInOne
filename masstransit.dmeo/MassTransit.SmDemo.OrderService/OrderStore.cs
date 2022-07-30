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

    public static void UpdateOrderState(string orderId, OrderState newState)
    {
        var order = GetOrder(orderId);
        order.State = newState;
        if (newState == OrderState.Paid)
        {
            order.PayTime = DateTime.Now;
        }
    }
}