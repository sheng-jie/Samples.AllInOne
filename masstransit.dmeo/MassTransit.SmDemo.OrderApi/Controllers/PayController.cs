using MassTransit.SmDemo.OrderApi.Models;
using MassTransit.SmDemo.Share.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MassTransit.SmDemo.OrderApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PayController : ControllerBase
{
    private readonly ILogger<PayController> _logger;
    private readonly IRequestClient<IGetOrderStateRequest> _getOrderRequestClient;
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public PayController(ILogger<PayController> logger, IRequestClient<IGetOrderStateRequest> getOrderRequestClient,
        ISendEndpointProvider sendEndpointProvider)
    {
        _logger = logger;
        _getOrderRequestClient = getOrderRequestClient;
        _sendEndpointProvider = sendEndpointProvider;
    }

    [HttpPost("{orderId}")]
    public async Task<IActionResult> Pay(string orderId, PayRequest payRequest)
    {
        if (orderId != payRequest.OrderId)
        {
            return BadRequest("无效请求");
        }

        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:payment"));

        await sendEndpoint.Send<IPayOrderRequest>(new
        {
            payRequest.UserId,
            payRequest.OrderId,
            payRequest.Amount
        });
        
        var (state, orderNotFound) =
            await _getOrderRequestClient.GetResponse<OrderLatestState, OrderNotFound>(new { OrderId = orderId });
        
        if (state.IsCompletedSuccessfully)
        {
            var orderState = await state;
            if (orderState.Message.Order.Status==OrderStatus.Paid)
            {
                return Ok($"订单已支付成功：{orderId}");
            }   
        }

        return BadRequest("订单不存在!");
    }
}