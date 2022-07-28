using System;
using System.Threading.Tasks;
using MassTransit;

namespace MassTransitDemo.MediatorService.Consumers
{
    public interface GetOrderStatus
    {
        Guid OrderId { get; }
    }

    public interface OrderStatus
    {
        Guid OrderId { get; }
        string Status { get; }
    }

    public class OrderStatusConsumer : IConsumer<GetOrderStatus>
    {
        public async Task Consume(ConsumeContext<GetOrderStatus> context)
        {
            Console.WriteLine($"OrderId:{context.Message.OrderId}");
            await context.RespondAsync<OrderStatus>(new
            {
                context.Message.OrderId,
                Status = "Pending"
            });
        }
    }
}
