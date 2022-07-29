namespace MassTransit.RequestResponseDemo;

public class Order
{
    public string OrderId { get; set; }

    public DateTime PaidTime { get; set; }

    public double Amount { get; set; }
}