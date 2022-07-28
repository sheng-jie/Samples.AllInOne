namespace MassTransit.RequestResponseDemo;

public interface IOrderRequest
{
    public string OrderId { get; set; }
}

public class OrderRequest:IOrderRequest
{
    public string OrderId { get; set; }

    public OrderRequest(string orderId)
    {
        OrderId = orderId;
    }
}

public interface IOrderResponse
{
    public Order Order { get; set; }
}

public class OrderResponse : IOrderResponse
{
    public Order Order { get; set; }
}

public class Order
{
    public string OrderId { get; set; }

    public DateTime PaidTime { get; set; }

    public double Amount { get; set; }
}