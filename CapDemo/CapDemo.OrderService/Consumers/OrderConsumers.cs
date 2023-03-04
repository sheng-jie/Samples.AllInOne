using CapDemo.OrderService.Data;
using CapDemo.OrderService.Domains;
using CapDemo.Shared;
using DotNetCore.CAP;

namespace CapDemo.OrderService.Consumers;

public class OrderConsumers:ICapSubscribe
{
    private readonly OrderDbContext _orderDbContext;
    private readonly ILogger<OrderConsumers> _logger;

    public OrderConsumers(OrderDbContext orderDbContext,ILogger<OrderConsumers> logger)
    {
        _orderDbContext = orderDbContext;
        _logger = logger;
    }
    [CapSubscribe(TopicConsts.CancelOrderCommand)]
    public async Task CancelOrder(string orderId)
    {
        if(string.IsNullOrEmpty(orderId)) return;
        var order = await _orderDbContext.Order.FindAsync(orderId);
        //幂等性设计
        if (order != null && order.Status != OrderStatus.Canceled)
        {
            order?.CancelOrder();
            _logger.LogWarning($"Order [{orderId}] has been canceled!");
            await _orderDbContext.SaveChangesAsync();
        }
    }

    [CapSubscribe(TopicConsts.PayOrderSucceedTopic)]
    public async  Task MarkToPaid(string orderId)
    {
        var order = await _orderDbContext.Order.FindAsync(orderId);
        //幂等性设计
        if (order != null && order.Status == OrderStatus.Pending)
        {
            order?.UpdateToPaid();
            await _orderDbContext.SaveChangesAsync();
        }
    }
}