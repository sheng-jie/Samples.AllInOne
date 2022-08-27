using MassTransit;
using MassTransit.Courier.Contracts;

namespace Masstransit.FirstActivity.Consumers;

public class RoutingSlipActivityConsumer :
    IConsumer<RoutingSlipActivityCompleted>,
    IConsumer<RoutingSlipActivityFaulted>,
    IConsumer<RoutingSlipActivityCompensated>,
    IConsumer<RoutingSlipActivityCompensationFailed>
{
    private readonly ILogger<RoutingSlipActivityConsumer> _logger;

    public RoutingSlipActivityConsumer(ILogger<RoutingSlipActivityConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<RoutingSlipActivityCompleted> context)
    {
        _logger.LogInformation("Routing Slip Activity Completed: {TrackingNumber} {ActivityName}",
            context.Message.TrackingNumber,
            context.Message.ActivityName);

        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<RoutingSlipActivityFaulted> context)
    {
        _logger.LogInformation("Routing Slip Activity Faulted: {TrackingNumber} {ExceptionInfo}",
            context.Message.TrackingNumber,
            context.Message.ExceptionInfo.Message);

        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<RoutingSlipActivityCompensated> context)
    {
        _logger.LogInformation("Routing Slip Activity Compensated: {TrackingNumber} {ActivityName}",
            context.Message.TrackingNumber,
            context.Message.ActivityName);

        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<RoutingSlipActivityCompensationFailed> context)
    {
        _logger.LogInformation("Routing Slip Activity Completed: {TrackingNumber} {ExceptionInfo}",
            context.Message.TrackingNumber,
            context.Message.ExceptionInfo.Message);

        return Task.CompletedTask;
    }
}