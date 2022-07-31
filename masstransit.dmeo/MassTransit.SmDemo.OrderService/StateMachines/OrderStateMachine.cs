using MassTransit.SmDemo.Share.Contracts;
using MassTransit.SmDemo.Share.Events;

namespace MassTransit.SmDemo.OrderService.StateMachines
{
    public class OrderStateMachine :
        MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            Event(() => OrderSubmitted, x => 
                x.CorrelateBy(m => m.Order.OrderId, x => x.Message.Order.OrderId));
            Event(() => OrderPaid, x =>
                x.CorrelateBy(m => m.Order.OrderId, x => x.Message.OrderId));
            Event(() => OrderCanceled, x =>
                x.CorrelateBy(m => m.Order.OrderId, x => x.Message.OrderId));
            
            Event(() => OrderShipped, x =>
                x.CorrelateBy(m => m.Order.OrderId, x => x.Message.OrderId));

            InstanceState(x => x.CurrentState);

            Initially(
                When(OrderSubmitted)
                    .Then(context =>
                    {
                        context.Saga.Updated = DateTime.Now;
                        context.Saga.Order = context.Message.Order;
                    })
                    .TransitionTo(Submitted));


            During(Submitted,
                Ignore(OrderSubmitted),
                When(OrderPaid)
                    .Then(context =>
                    {
                        context.Saga.Updated = DateTime.Now;
                        context.Saga.Order.PayTime = DateTime.Now;
                        context.Saga.Order.Status = OrderStatus.Paid;
                    })
                    .TransitionTo(Paid),
                When(OrderCanceled)
                    .Then(context =>
                    {
                        context.Saga.Updated = DateTime.Now;
                        context.Saga.Order.Status = OrderStatus.Canceled;
                    }).TransitionTo(Canceled));


            During(Paid,
                Ignore(OrderPaid),
                When(OrderShipped)
                    .Then(context =>
                    {
                        context.Saga.Updated = DateTime.Now;
                        context.Saga.Order.Status = OrderStatus.Shipped;
                    }).TransitionTo(Shipped));

            SetCompleted(async instance =>
            {
                var currentState = await this.GetState(instance);

                return Shipped.Equals(currentState);
            });
        }

        public State Submitted { get; private set; }

        public State Paid { get; private set; }
        public State Shipped { get; private set; }
        public State Canceled { get; private set; }

        public State Completed { get; private set; }

        public Event<IOrderSubmittedEvent> OrderSubmitted { get; private set; }
        public Event<IOrderPaidEvent> OrderPaid { get; private set; }

        public Event<IOrderCanceledEvent> OrderCanceled { get; private set; }

        public Event<IOrderShippedEvent> OrderShipped { get; private set; }
    }
}