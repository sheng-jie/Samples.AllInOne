namespace MassTransit.SmDemo.Shared.Contracts;

public interface ICancelOrderCommand
{
    public Guid OrderId { get; set; }
}