namespace MassTransit.SmDemo.Share.Contracts;

public interface IPayOrderRequest 
{
    public string UserId { get; set; }
    public string OrderId { get; set; }
    public double Amount { get; set; }
}