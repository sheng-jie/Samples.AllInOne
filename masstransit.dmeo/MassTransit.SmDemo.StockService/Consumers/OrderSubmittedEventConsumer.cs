using MassTransit.SmDemo.Share.Events;

namespace MassTransit.SmDemo.StockService.Consumers;

public class OrderSubmittedEventConsumer:IConsumer<OrderSubmittedEvent>
{
    public Task Consume(ConsumeContext<OrderSubmittedEvent> context)
    {
        // 扣减库存
        return Task.CompletedTask;
    }
}