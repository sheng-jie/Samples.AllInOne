using Automatonymous;

namespace MassTransit.SmDemo.OrderService.StateMachines.OrderStateMachineActivities
{

    // public class AcceptOrderActivity :
    //     Activity<OrderState, OrderAccepted>
    // {
    //     public void Probe(ProbeContext context)
    //     {
    //         context.CreateScope("accept-order");
    //     }
    //
    //     public void Accept(StateMachineVisitor visitor)
    //     {
    //         visitor.Visit(this);
    //     }
    //
    //     public async Task Execute(BehaviorContext<OrderState, OrderAccepted> context, Behavior<OrderState, OrderAccepted> next)
    //     {
    //         Console.WriteLine("Hello, World. Order is {0}", context.Data.OrderId);
    //
    //         var consumeContext = context.GetPayload<ConsumeContext>();
    //
    //         var sendEndpoint = await consumeContext.GetSendEndpoint(new Uri("queue:fulfill-order"));
    //
    //         await sendEndpoint.Send<FulfillOrder>(new
    //         {
    //             context.Data.OrderId,
    //             CustomerNumber = context.Instance.UserId,
    //             context.Instance.PaymentCardNumber,
    //         });
    //
    //         await next.Execute(context).ConfigureAwait(false);
    //     }
    //
    //     public Task Faulted<TException>(BehaviorExceptionContext<OrderState, OrderAccepted, TException> context, Behavior<OrderState, OrderAccepted> next)
    //         where TException : Exception
    //     {
    //         return next.Faulted(context);
    //     }
    // }
}