namespace MassTransit.SmDemo.Shared.Contracts;

public interface IPayOrderCommand
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
}

public interface IOrderStateRequest
{
    public Guid OrderId { get; set; }
}

public interface IOrderStateResponse
{
    public Guid OrderId { get; set; }
    public string State { get; set; }
}

public interface IOrderNotFoundOrCompleted
{
    public Guid OrderId { get; set; }
}