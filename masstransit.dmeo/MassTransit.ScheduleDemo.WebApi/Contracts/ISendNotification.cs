namespace MassTransit.ScheduleDemo.WebApi.Contracts;

public interface ISendNotification
{
    string EmailAddress { get; }
    string Body { get; }
}