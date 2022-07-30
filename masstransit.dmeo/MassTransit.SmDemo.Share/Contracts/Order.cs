namespace MassTransit.SmDemo.Share.Contracts;

public class Order
{
    public string OrderId { get;  }
    public List<OrderItem> OrderItems { get; }

    public Order(string userId, List<OrderItem> orderItems )
    {
        UserId = userId;
        OrderId = Guid.NewGuid().ToString();
        OrderItems = orderItems;
        CreatTime = DateTime.Now;
        Amount = orderItems.Sum(item => item.Num * item.Price);
        State = OrderState.Submitted;
    }
    public string UserId { get; }
    public DateTime CreatTime { get;  }
    
    public DateTime? PayTime { get; set; }
    public double Amount { get;  }
    public OrderState State { get; set; }
}