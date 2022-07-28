namespace MassTransit.PublishDemo.Contracts
{
    public record ValueChangedEvent
    {
        public string Value { get; init; }
    }

}