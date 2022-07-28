using MassTransit.RequestResponseDemo.Contracts;

namespace MassTransit.RequestResponseDemo;

public class OrderRquestConsumer:IConsumer<IOrderRequest>
{
    public Task Consume(ConsumeContext<IOrderRequest> context)
    {
        throw new NotImplementedException();
    }
}