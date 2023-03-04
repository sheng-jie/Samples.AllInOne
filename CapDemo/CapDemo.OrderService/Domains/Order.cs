namespace CapDemo.OrderService.Domains;

public class Order
{
    private Order()
    {
        
    }
    public Order(string customerId)
    {
        CustomerId = customerId;
    }
    
    
    public string OrderId { get; private set; }
    public string CustomerId { get; private set; }

    public decimal Amount { get; private set; }

    public List<OrderItem> OrderItems { get; private set; }

    public DateTime CreatedTime { get; private set; }

    public OrderStatus Status { get; private set; }

    public Order NewOrder(ShoppingCartItem[] shoppingCartItems)
    {
        if (!shoppingCartItems.Any())
            throw new InvalidDataException("无效订单数据项");
        this.OrderItems = new List<OrderItem>();
        this.OrderId = Guid.NewGuid().ToString();
        foreach (var shoppingCartItem in shoppingCartItems)
        {
            OrderItems.Add(new OrderItem(OrderId, shoppingCartItem.SkuId, shoppingCartItem.Price,
                shoppingCartItem.Qty));
        }

        this.Amount = OrderItems.Sum(s => s.Price * s.Qty);
        this.Status = OrderStatus.Pending;
        this.CreatedTime = DateTime.Now;

        return this;
    }
    

    public void CancelOrder()
    {
        this.Status = OrderStatus.Canceled;
    }
    public void UpdateToPaid()
    {
        this.Status = OrderStatus.Paid;
    }
}