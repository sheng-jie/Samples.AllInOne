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
                x.CorrelateById(m => m.Message.Order.OrderId));
            Event(() => OrderPaid, x =>
                x.CorrelateById(m => m.Message.OrderId));
            Event(() => OrderCanceled, x =>
                x.CorrelateById(m => m.Message.OrderId));
            
            Event(() => OrderShipped, x =>
                x.CorrelateById(m => m.Message.OrderId));
            
            Event(() => OrderStateRequested, x =>
            {
                x.CorrelateById(m => m.Message.OrderId);
                x.OnMissingInstance(m => m.ExecuteAsync(async context =>
                {
                    if (context.RequestId.HasValue)
                    {
                        await context.RespondAsync<OrderNotFound>(new {context.Message.OrderId});
                    }
                }));
            });
            
            // Event(() => OrderSubmitted, x => 
            //     x.CorrelateBy(m => m.Order.OrderId, x => x.Message.Order.OrderId));
            // Event(() => OrderPaid, x =>
            //     x.CorrelateBy(m => m.Order.OrderId, x => x.Message.OrderId));
            // Event(() => OrderCanceled, x =>
            //     x.CorrelateBy(m => m.Order.OrderId, x => x.Message.OrderId));
            //
            // Event(() => OrderShipped, x =>
            //     x.CorrelateBy(m => m.Order.OrderId, x => x.Message.OrderId));

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
            
            DuringAny(
                When(OrderStateRequested)
                    .RespondAsync(x => x.Init<OrderLatestState>(new OrderLatestState()
                    {
                        OrderId = x.Saga.CorrelationId,
                        LastUpdateTime = x.Saga.Updated,
                        Order = x.Saga.Order,
                        State = x.Saga.CurrentState
                    }))
            );
        }

        public State Submitted { get; private set; }

        public State Paid { get; private set; }
        public State Shipped { get; private set; }
        public State Canceled { get; private set; }

        public State Completed { get; private set; }

        public Event<IOrderSubmittedEvent> OrderSubmitted { get; private set; }
        
        public Event<IGetOrderStateRequest> OrderStateRequested { get; private set; }
        public Event<IOrderPaidEvent> OrderPaid { get; private set; }

        public Event<IOrderCanceledEvent> OrderCanceled { get; private set; }

        public Event<IOrderShippedEvent> OrderShipped { get; private set; }
    }
}