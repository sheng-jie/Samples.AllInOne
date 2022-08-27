using MassTransit;
using MassTransit.Courier.Contracts;

namespace Masstransit.FirstActivity.Consumers;

public class RoutingSlipConsumer :
    IConsumer<RoutingSlipCompleted>,
    IConsumer<RoutingSlipFaulted>,
    IConsumer<RoutingSlipCompensationFailed>
{
    private readonly ILogger<RoutingSlipConsumer> _logger;

    public RoutingSlipConsumer(ILogger<RoutingSlipConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<RoutingSlipCompleted> context)
    {
        _logger.LogInformation("Routing Slip Completed: {TrackingNumber} {ActivityName}",
            context.Message.TrackingNumber,
            context.Message.Duration);
        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<RoutingSlipFaulted> context)
    {
        _logger.LogInformation("Routing Slip Faulted: {TrackingNumber} {ExceptionInfo}", context.Message.TrackingNumber,
            context.Message.ActivityExceptions.FirstOrDefault());
        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<RoutingSlipCompensationFailed> context)
    {
        _logger.LogInformation("Routing Slip compensation failed: {TrackingNumber} {ExceptionInfo}",
            context.Message.TrackingNumber,
            context.Message.ExceptionInfo.Message);
        return Task.CompletedTask;
    }
}