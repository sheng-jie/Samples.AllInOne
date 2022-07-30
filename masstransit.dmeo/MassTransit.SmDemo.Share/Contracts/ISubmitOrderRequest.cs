namespace MassTransit.SmDemo.Share.Contracts;

public interface ISubmitOrderRequest
{
    public string UserId { get; set; }
    public List<OrderItem> OrderItems { get; set; }
}