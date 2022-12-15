using MassTransit.CourierDemo.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace MassTransit.CourierDemo.OrderService.Controllers;
[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IBus _bus;
    public OrderController(IBus bus)
    {
        _bus = bus;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderDto createOrderDto)
    {
        var orderRoutingSlip = OrderRoutingSlipBuilder.BuildOrderRoutingSlip(createOrderDto);
        await _bus.Execute(orderRoutingSlip);

        return Ok();
    }
}