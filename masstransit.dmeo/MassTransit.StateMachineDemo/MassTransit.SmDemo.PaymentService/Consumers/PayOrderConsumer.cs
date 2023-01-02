using MassTransit.SmDemo.Shared.Contracts;

namespace MassTransit.SmDemo.PaymentService.Consumers;

public class PayOrderConsumer : IConsumer<IPayOrderCommand>
{
    private readonly ILogger<PayOrderConsumer> _logger;

    public PayOrderConsumer(ILogger<PayOrderConsumer> logger)
    {
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<IPayOrderCommand> context)
    {
        await Task.Delay(TimeSpan.FromSeconds(10));
        if (context.Message.Amount % 2 == 0)
        {_logger.LogInformation($"Order [{context.Message.OrderId}] paid successfully!");
            await context.Publish<IPayOrderSucceed>(new { context.Message.OrderId });
        }
        else
        {
            _logger.LogWarning($"Order [{context.Message.OrderId}] payment failed!");
            await context.Publish<IPayOrderFailed>(new
            {
                context.Message.OrderId,
                Reason = "Insufficient account balance"
            });
        }
    }
}