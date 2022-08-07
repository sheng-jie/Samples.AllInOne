using MassTransit.ScheduleDemo.WebApi.Contracts;

namespace MassTransit.ScheduleDemo.WebApi.Consumers;

public class SendNotificationConsumer : IConsumer<ISendNotification>
{
    private readonly ILogger<ISendNotification> _logger;

    public SendNotificationConsumer(ILogger<ISendNotification> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<ISendNotification> context)
    {
        _logger.LogInformation($"Message sent to {context.Message.EmailAddress} successfully!");
        
        return Task.CompletedTask;
    }
}