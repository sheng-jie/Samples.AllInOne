using MassTransit.ScheduleDemo.WebApi.Contracts;
using MassTransit.ScheduleDemo.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace MassTransit.ScheduleDemo.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
    private readonly ILogger<NotificationController> _logger;
    private readonly IBus _bus;
    private readonly IMessageScheduler _scheduler;

    public NotificationController(ILogger<NotificationController> logger,IBus bus,IMessageScheduler scheduler)
    {
        _logger = logger;
        _bus = bus;
        _scheduler = scheduler;
    }

    [HttpPost]
    public async Task<IActionResult> Post(NotificationMessage message)
    {
        _logger.LogInformation("send a new message !");
        await _bus.Publish<ISendNotification>(new 
            {
                EmailAddress = message.EmailAddress,
                Body =  message.Body
            });

        return Ok();
    }
    
    [HttpPost("[action]")]
    public async Task<IActionResult> Schedule(NotificationMessage message)
    {
        _logger.LogInformation("Scheduled a new message !");
        await _scheduler.SchedulePublish<IScheduleNotification>(DateTime.Now.AddSeconds(10), new 
        {
            DeliveryTime = DateTime.Now.AddSeconds(10),
            EmailAddress = message.EmailAddress,
            Body =  message.Body
        });

        return Ok();
    }
}