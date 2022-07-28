

using MassTransit.RequestResponseDemo;

namespace MassTransit.RpDemo.Response;

public class OrderRequestConsumer : IConsumer<IOrderRequest>
{
    public async Task Consume(ConsumeContext<IOrderRequest> context)
    {
        Console.WriteLine($"Receive order request:{context.Message.OrderId}");
        await context.RespondAsync<IOrderResponse>(new OrderResponse()
        {
            Order = new Order()
            {
                OrderId = context.Message.OrderId,
                Amount = DateTime.Now.Millisecond,
                PaidTime = DateTime.Now.AddHours(-10)
            }
        });
    }
}