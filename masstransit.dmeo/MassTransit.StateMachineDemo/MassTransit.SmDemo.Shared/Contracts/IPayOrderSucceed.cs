namespace MassTransit.SmDemo.Shared.Contracts;

public interface IPayOrderSucceed
{
    public Guid OrderId { get; set; }
}