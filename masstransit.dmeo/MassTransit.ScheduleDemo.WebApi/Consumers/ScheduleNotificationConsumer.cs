using MassTransit.ScheduleDemo.WebApi.Contracts;

namespace MassTransit.ScheduleDemo.WebApi.Consumers;

public class ScheduleNotificationConsumer : IConsumer<IScheduleNotification>
{
    private readonly ILogger<ScheduleNotificationConsumer> _logger;

    public ScheduleNotificationConsumer(ILogger<ScheduleNotificationConsumer> logger)
    {
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<IScheduleNotification> context)
    {
        Uri notificationService = new Uri("queue:send-notification");
        
        await context.ScheduleSend<ISendNotification>(notificationService,
            context.Message.DeliveryTime,
            new
            {
                EmailAddress = context.Message.EmailAddress,
                Body = context.Message.Body
            });
        
        _logger.LogInformation("Scheduled Succeed!");
    }
}