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
                        await context.RespondAsync<OrderNotFound>(new { context.Message.OrderId });
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
                    .TransitionTo(Paid));

            During(Paid,
                Ignore(OrderPaid),
                When(OrderCanceled)
                    .Then(context =>
                    {
                        context.Saga.Updated = DateTime.Now;
                        context.Saga.Order.Status = OrderStatus.Canceled;
                    }).TransitionTo(Canceled).Finalize(),//当订单已取消,则订单结束,标记为最终状态
                When(OrderShipped)
                    .Then(context =>
                    {
                        context.Saga.Updated = DateTime.Now;
                        context.Saga.Order.Status = OrderStatus.Shipped;
                    }).TransitionTo(Shipped));


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

            //当流程到达终点时,标记状态机完成,并移除状态机
            SetCompletedWhenFinalized();
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