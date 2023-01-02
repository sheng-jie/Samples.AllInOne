namespace MassTransit.SmDemo.Shared.Contracts;

public interface IDeduceInventorySucceed
{
    public Guid OrderId { get; set; }
}