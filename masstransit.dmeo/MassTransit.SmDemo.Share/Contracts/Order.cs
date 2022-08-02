namespace MassTransit.SmDemo.Share.Contracts;

public class Order
{
    public Guid OrderId { get;  }
    public List<OrderItem> OrderItems { get; set; }

    public Order(Guid orderId,string userId, List<OrderItem> orderItems )
    {
        OrderId = orderId;
        UserId = userId;
        OrderItems = orderItems;
        CreatTime = DateTime.Now;
        Amount = orderItems.Sum(item => item.Num * item.Price);
        Status = OrderStatus.Submitted;
    }
    public string UserId { get; private set; }
    public DateTime CreatTime { get; private set; }

    public DateTime? PayTime { get; set; }
    public double Amount { get; private set; }
    public OrderStatus Status { get; set; }
}