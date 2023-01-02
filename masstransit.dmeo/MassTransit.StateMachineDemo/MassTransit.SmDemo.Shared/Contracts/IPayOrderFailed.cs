namespace MassTransit.SmDemo.Shared.Contracts;

public interface IPayOrderFailed
{
    public Guid OrderId { get; set; }
    public string Reason { get; set; }
}