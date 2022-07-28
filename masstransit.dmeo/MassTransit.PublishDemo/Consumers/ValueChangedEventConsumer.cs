using System.Threading.Tasks;
using MassTransit.PublishDemo.Contracts;
using Microsoft.Extensions.Logging;

namespace MassTransit.PublishDemo.Consumers
{
    public class ValueChangedEventConsumer :
        IConsumer<ValueChangedEvent>
    {
        private readonly ILogger<ValueChangedEventConsumer> _logger;

        public ValueChangedEventConsumer(ILogger<ValueChangedEventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<ValueChangedEvent> context)
        {
            _logger.LogInformation("Received Text: {Text}", context.Message.Value);
            return Task.CompletedTask;
        }
    }
}