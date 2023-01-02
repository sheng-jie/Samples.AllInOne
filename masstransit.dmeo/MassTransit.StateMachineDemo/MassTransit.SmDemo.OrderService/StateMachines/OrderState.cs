using MassTransit.SmDemo.OrderService.Domains;

namespace MassTransit.SmDemo.OrderService;

public class OrderState :
    SagaStateMachineInstance,
    ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }
    public string CurrentState { get; set; }
    
    public Guid OrderId { get; set; }

    public decimal Amount { get; set; }

    public List<OrderItem> OrderItems { get; set; }

}