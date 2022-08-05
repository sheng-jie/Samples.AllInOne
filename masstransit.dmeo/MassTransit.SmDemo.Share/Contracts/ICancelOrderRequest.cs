namespace MassTransit.SmDemo.Share.Contracts;

public interface ICancelOrderRequest 
{
    public Guid OrderId { get; set; }
}