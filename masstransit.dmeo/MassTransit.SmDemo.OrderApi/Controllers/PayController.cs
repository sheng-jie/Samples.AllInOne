using MassTransit.SmDemo.OrderApi.Models;
using MassTransit.SmDemo.Share.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MassTransit.SmDemo.OrderApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PayController : ControllerBase
{
    private readonly ILogger<PayController> _logger;
    private readonly IRequestClient<IGetOrderRequest> _getOrderRequestClient;
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public PayController(ILogger<PayController> logger,IRequestClient<IGetOrderRequest> getOrderRequestClient, ISendEndpointProvider sendEndpointProvider)
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
        
        var orderResponse =await _getOrderRequestClient.GetResponse<Order>(new { OrderId = orderId });
        
        if (orderResponse.Message == null) return BadRequest("订单不存在!");
        
        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:payment"));

        await sendEndpoint.Send<IPayOrderRequest>(new
        {
            payRequest.UserId,
            payRequest.OrderId,
            payRequest.Amount
        });
        return Ok();
    }
}