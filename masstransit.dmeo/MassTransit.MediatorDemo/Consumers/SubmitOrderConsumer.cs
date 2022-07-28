using System;
using System.Threading.Tasks;
using MassTransit;

namespace MassTransitDemo.MediatorService.Consumers
{
    public interface SubmitOrder
    {
        Guid ProductId { get; }
    }

    public class SubmitOrderConsumer : IConsumer<SubmitOrder>
    {
        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            Console.WriteLine($"ProductId:{context.Message.ProductId}");
            await context.ConsumeCompleted;
        }
    }
}
