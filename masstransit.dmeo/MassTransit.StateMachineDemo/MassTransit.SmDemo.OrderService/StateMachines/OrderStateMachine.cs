using MassTransit.SmDemo.OrderService.Events;
using MassTransit.SmDemo.Shared.Contracts;

namespace MassTransit.SmDemo.OrderService;

public class OrderStateMachine :
    MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        Event(() => OrderCreated, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => DeduceInventorySucceed, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => DeduceInventoryFailed, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => ReturnInventorySucceed, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => PayOrderSucceed, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => PayOrderFailed, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => OrderCanceled, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => OrderStateRequested, x =>
        {
            x.CorrelateById(m => m.Message.OrderId);
            x.OnMissingInstance(m =>
            {
                return m.ExecuteAsync(x => x.RespondAsync<IOrderNotFoundOrCompleted>(new { x.Message.OrderId }));
            });
        });

        InstanceState(x => x.CurrentState);

        Initially(
            When(OrderCreated)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.OrderItems = context.Message.OrderItems;
                    context.Saga.Amount = context.Message.OrderItems.Sum(x => x.Price * x.Qty);
                })
                .PublishAsync(context => context.Init<IDeduceInventoryCommand>(new
                {
                    context.Saga.OrderId,
                    DeduceInventoryItems =
                        context.Saga.OrderItems.Select(x => new DeduceInventoryItem(x.SkuId, x.Qty)).ToList()
                }))
                .TransitionTo(Created));

        During(Created,
            When(DeduceInventorySucceed)
                .Then(context =>
                {
                    context.Publish<IPayOrderCommand>(new
                    {
                        context.Saga.OrderId,
                        context.Saga.Amount
                    });
                }).TransitionTo(InventoryDeducted),
            When(DeduceInventoryFailed).Then(context =>
            {
                context.Publish<ICancelOrderCommand>(new
                {
                    context.Saga.OrderId
                });
            })
        );

        During(InventoryDeducted,
            When(PayOrderFailed).Then(context =>
            {
                context.Publish<IReturnInventoryCommand>(new
                {
                    context.Message.OrderId,
                    ReturnInventoryItems =
                        context.Saga.OrderItems.Select(x => new ReturnInventoryItem(x.SkuId, x.Qty)).ToList()
                });
            }),
            When(PayOrderSucceed).TransitionTo(Paid).Then(context => context.SetCompleted()),
            When(ReturnInventorySucceed)
                .ThenAsync(context => context.Publish<ICancelOrderCommand>(new
                {
                    context.Saga.OrderId
                })).TransitionTo(Created));

        DuringAny(When(OrderCanceled).TransitionTo(Canceled).ThenAsync(async context =>
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            await context.SetCompleted();
        }));


        DuringAny(
            When(OrderStateRequested)
                .RespondAsync(x => x.Init<IOrderStateResponse>(new
                {
                    x.Saga.OrderId,
                    State = x.Saga.CurrentState
                }))
        );

        //当流程到达终点时,标记状态机完成,并移除状态机
        // SetCompletedWhenFinalized();
    }

    public State Created { get; private set; }

    public State InventoryDeducted { get; private set; }
    public State Paid { get; private set; }
    public State Canceled { get; private set; }

    public Event<ICreateOrderSucceed> OrderCreated { get; private set; }

    public Event<IDeduceInventorySucceed> DeduceInventorySucceed { get; private set; }
    public Event<IDeduceInventoryFailed> DeduceInventoryFailed { get; private set; }

    public Event<ICancelOrderSucceed> OrderCanceled { get; private set; }

    public Event<IPayOrderSucceed> PayOrderSucceed { get; private set; }
    public Event<IPayOrderFailed> PayOrderFailed { get; private set; }

    public Event<IReturnInventorySucceed> ReturnInventorySucceed { get; private set; }

    public Event<IOrderStateRequest> OrderStateRequested { get; private set; }
}