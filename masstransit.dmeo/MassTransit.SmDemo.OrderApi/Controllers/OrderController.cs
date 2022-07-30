using MassTransit.SmDemo.OrderApi.Models;
using MassTransit.SmDemo.Share.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MassTransit.SmDemo.OrderApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IRequestClient<ISubmitOrderRequest> _submitOrderRequestClient;
    // private readonly Logger<OrderController> _logger;

    public OrderController(IRequestClient<ISubmitOrderRequest> submitOrderRequestClient)
    {
        _submitOrderRequestClient = submitOrderRequestClient;
        // _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get(string orderId)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<IActionResult> Post(OrderRequest request)
    {
        var (accepted, rejected) = await _submitOrderRequestClient.GetResponse<OrderSubmitSucceed, OrderSubmitRejected>(
            new
            {
                UserId = "shengjie",
                OrderItems = new List<OrderItem>()
                {
                    new OrderItem("iphone 14", 1, 5000),
                    new OrderItem("iphone 14 charge", 1, 100)
                }
            });

        if (accepted.IsCompletedSuccessfully)
        {
            var response = await accepted;
            // _logger.LogWarning($"Submit order succeed, orderId:{response.Message.Order.OrderId}, amount:{response.Message.Order.Amount}");
            return Accepted(response);
        }
        else
        {
            var response = await rejected;
            // _logger.LogWarning($"Submit order failed:{response.Message.Reason}");

            return BadRequest(response.Message.Reason);
        }
    }
}