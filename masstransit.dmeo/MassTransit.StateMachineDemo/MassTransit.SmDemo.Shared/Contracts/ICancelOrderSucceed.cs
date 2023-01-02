namespace MassTransit.SmDemo.Shared.Contracts;

public interface ICancelOrderSucceed
{
    public Guid OrderId { get; set; }
}