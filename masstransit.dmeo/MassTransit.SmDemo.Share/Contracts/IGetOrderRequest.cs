namespace MassTransit.SmDemo.Share.Contracts;

public interface IGetOrderRequest 
{
    public string OrderId { get; set; }
}
