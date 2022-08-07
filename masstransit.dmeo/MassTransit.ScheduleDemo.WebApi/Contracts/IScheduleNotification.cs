namespace MassTransit.ScheduleDemo.WebApi.Contracts;

public interface IScheduleNotification
{
    DateTime DeliveryTime { get; }
    string EmailAddress { get; }
    string Body { get; }
}