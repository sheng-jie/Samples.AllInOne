namespace MassTransit.RequestResponseDemo.Contracts;

public interface IGetOrderRequest
{
    public string OrderId { get; set; }
}

public interface IOrderResponse
{
    public Order Order { get; set; }
}

public class Order
{
    public string OrderId { get; set; }

    public DateTime PaidTime { get; set; }

    public double Amount { get; set; }

    public int Status { get; set; }
}