using MassTransit.SmDemo.OrderService.Events;
using MassTransit.SmDemo.OrderService.Models;
using MassTransit.SmDemo.Shared.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MassTransit.SmDemo.OrderService.Controllers;
[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IBus _bus;
    private readonly IRequestClient<IOrderStateRequest> _client;

    public OrderController(IBus bus,IRequestClient<IOrderStateRequest> client)
    {
        _bus = bus;
        _client = client;
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid orderId)
    {
        var (status,notFound) = await _client.GetResponse<IOrderStateResponse,IOrderNotFoundOrCompleted>(new { orderId });
        if (status.IsCompletedSuccessfully)
        {
            var response = await status;
            return Ok(response.Message);
        }
        else
        {
            var response = await notFound;
            return NotFound($"Order {response.Message.OrderId} does not exists or has been completed!");
        };
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderDto createOrderDto)
    {
        await _bus.Publish<ICreateOrderCommand>(new
        {
            createOrderDto.CustomerId,
            createOrderDto.ShoppingCartItems
        });
        return Ok();
    }
}