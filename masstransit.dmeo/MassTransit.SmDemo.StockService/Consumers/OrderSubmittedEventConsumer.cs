using MassTransit.SmDemo.Share.Events;

namespace MassTransit.SmDemo.StockService.Consumers;

public class OrderSubmittedEventConsumer:IConsumer<IOrderSubmittedEvent>
{
    public Task Consume(ConsumeContext<IOrderSubmittedEvent> context)
    {
        // 扣减库存
        return Task.CompletedTask;
    }
}