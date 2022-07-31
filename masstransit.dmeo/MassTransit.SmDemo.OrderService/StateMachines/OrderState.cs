using MassTransit.SmDemo.Share.Contracts;

namespace MassTransit.SmDemo.OrderService.StateMachines
{
    public class OrderState :
        SagaStateMachineInstance,
        ISagaVersion
    {
        public string CurrentState { get; set; }

        public Order Order { get; set; }

        public int Version { get; set; }

        public Guid CorrelationId { get; set; }

        public DateTime Updated { get; set; }
    }
}