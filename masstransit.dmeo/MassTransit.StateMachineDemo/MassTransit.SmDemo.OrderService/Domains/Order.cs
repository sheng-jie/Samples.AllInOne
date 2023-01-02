namespace MassTransit.SmDemo.OrderService.Domains;

public class Order
{
    public Order(string customerId)
    {
        this.CustomerId = customerId;
    }
    public Guid OrderId { get; private set; }
    public string CustomerId { get; private set; }

    public decimal Amount { get; private set; }

    public List<OrderItem> OrderItems { get; private set; }

    public DateTime CreatedTime { get; private set; }
    public DateTime PaidTime { get; set; }

    public OrderStatus Status { get;  set; }

    public Order NewOrder(ShoppingCartItem[] shoppingCartItems)
    {
        if (!shoppingCartItems.Any())
            throw new InvalidDataException("无效订单数据项");
        this.OrderItems = new List<OrderItem>();
        this.OrderId = Guid.NewGuid();
        foreach (var shoppingCartItem in shoppingCartItems)
        {
            OrderItems.Add(new OrderItem(OrderId, shoppingCartItem.SkuId, shoppingCartItem.Price,
                shoppingCartItem.Qty));
        }

        this.Amount = OrderItems.Sum(s => s.Price * s.Qty);
        this.Status = OrderStatus.Submitted;
        this.CreatedTime = DateTime.Now;

        return this;
    }
    

    public void CancelOrder()
    {
        this.Status = OrderStatus.Canceled;
    }
}