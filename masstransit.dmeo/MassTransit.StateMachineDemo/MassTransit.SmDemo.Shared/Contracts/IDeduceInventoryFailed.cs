namespace MassTransit.SmDemo.Shared.Contracts;

public interface IDeduceInventoryFailed
{
    public Guid OrderId { get; set; }
    public string Reason { get; set; }
}