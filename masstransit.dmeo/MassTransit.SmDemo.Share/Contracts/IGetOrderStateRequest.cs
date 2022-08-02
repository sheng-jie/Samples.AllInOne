namespace MassTransit.SmDemo.Share.Contracts;

public interface IGetOrderStateRequest 
{
    public Guid OrderId { get; set; }
}

public class OrderLatestState
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; }

    public string State { get; set; }

    public DateTime LastUpdateTime { get; set; }
}

public interface OrderNotFound
{
    Guid OrderId { get; }
}