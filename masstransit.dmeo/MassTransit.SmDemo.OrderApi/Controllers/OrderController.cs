using MassTransit.SmDemo.OrderApi.Models;
using MassTransit.SmDemo.Share.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MassTransit.SmDemo.OrderApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IRequestClient<IGetOrderStateRequest> _getOrderStateClient;
    private readonly ISendEndpointProvider _provider;
    private readonly IRequestClient<ISubmitOrderRequest> _submitOrderRequestClient;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IRequestClient<ISubmitOrderRequest> submitOrderRequestClient,
        ILogger<OrderController> logger,
        IRequestClient<IGetOrderStateRequest> getOrderStateClient, ISendEndpointProvider provider)
    {
        _submitOrderRequestClient = submitOrderRequestClient;
        _logger = logger;
        _getOrderStateClient = getOrderStateClient;
        _provider = provider;
    }

    [HttpGet]
    public async Task<IActionResult> Get(string orderId)
    {
        var (state, orderNotFound) =
            await _getOrderStateClient.GetResponse<OrderLatestState, OrderNotFound>(new { OrderId = orderId });
        if (state.IsCompletedSuccessfully)
        {
            var orderstate = await state;
            return Ok(orderstate.Message);
        }

        return NotFound(orderId);
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
            _logger.LogWarning(
                $"Submit order succeed, orderId:{response.Message.Order.OrderId}, amount:{response.Message.Order.Amount}");
            return Accepted(response);
        }
        else
        {
            var response = await rejected;
            _logger.LogWarning($"Submit order failed:{response.Message.Reason}");

            return BadRequest(response.Message.Reason);
        }
    }

    [HttpPut("{orderId}/[action]")]
    public async Task<IActionResult> Cancel(string orderId)
    {
        var sendEndpoint = await _provider.GetSendEndpoint(new Uri("queue:cancel-order"));

        await sendEndpoint.Send<ICancelOrderRequest>(new
        {
            OrderId = orderId
        });

        return Ok("已提交订单取消申请!");
    }
}