using MassTransit.ScheduleDemo.WebApi.Contracts;

namespace MassTransit.ScheduleDemo.WebApi.Models;

public class NotificationMessage : ISendNotification
{
    public string EmailAddress { get; set; }
    public string Body { get; set; }
}