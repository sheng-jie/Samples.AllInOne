using MassTransit.SmDemo.Share.Events;
using Microsoft.AspNetCore.Mvc;

namespace MassTransit.SmDemo.OrderApi.Controllers;

[ApiController]
[Route("[Controller]")]
public class DeliveryController : ControllerBase
{
    private readonly IBus _bus;

    public DeliveryController(IBus bus)
    {
        _bus = bus;
    }
    [HttpPost("{orderId}")]
    public async Task<IActionResult> Post(Guid orderId)
    {
        //do some logic here
        //...
        await _bus.Publish<IOrderShippedEvent>(new { OrderId = orderId });

        return Ok($"订单已发货:{orderId}");

    }
}