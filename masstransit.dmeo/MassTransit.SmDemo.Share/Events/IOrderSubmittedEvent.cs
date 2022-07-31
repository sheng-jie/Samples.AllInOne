using MassTransit.SmDemo.Share.Contracts;

namespace MassTransit.SmDemo.Share.Events;

public interface IOrderSubmittedEvent
{
    public Order Order { get; set; }
}