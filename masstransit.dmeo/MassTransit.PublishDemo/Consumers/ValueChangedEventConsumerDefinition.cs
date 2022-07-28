namespace MassTransit.PublishDemo.Consumers
{
    public class ValueChangedEventConsumerDefinition :
        ConsumerDefinition<ValueChangedEventConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ValueChangedEventConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
        }
    }
}