using MassTransit.CourierDemo.OrderService.Domains;
using MassTransit.CourierDemo.OrderService.Repositories;
using MassTransit.CourierDemo.Shared.Models;

namespace MassTransit.CourierDemo.OrderService.Activities;

public class CreateOrderActivity : IActivity<CreateOrderDto, CreateOrderLog>
{
    private readonly ILogger<CreateOrderActivity> _logger;
    public CreateOrderActivity(ILogger<CreateOrderActivity> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 订单创建
    /// </summary>
    public async Task<ExecutionResult> Execute(ExecuteContext<CreateOrderDto> context)
    {
        var order = await CreateOrder(context.Arguments);
        var log = new CreateOrderLog(order.OrderId, order.CreatedTime);
        _logger.LogInformation($"Order [{order.OrderId}] created successfully!");
        return context.CompletedWithVariables(log, new {order.OrderId});
    }

    private async Task<Order> CreateOrder(CreateOrderDto orderDto)
    {
        var shoppingItems =
            orderDto.ShoppingCartItems.Select(item => new ShoppingCartItem(item.SkuId, item.Price, item.Qty));
        var order = new Order(orderDto.CustomerId).NewOrder(shoppingItems.ToArray());
        await OrderRepository.Insert(order);
        return order;
    }

    /// <summary>
    /// 订单补偿（取消订单）
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<CompensationResult> Compensate(CompensateContext<CreateOrderLog> context)
    {
        var order = await OrderRepository.Get(context.Log.OrderId);
        order.CancelOrder();
        var exception = context.Message.ActivityExceptions.FirstOrDefault();

        _logger.LogWarning(
            $"Order [{order.OrderId} has been canceled!({exception.ExceptionInfo.Message})");

        return context.Compensated();
    }
}

public class CreateOrderLog
{
    public string OrderId { get; private set; }
    public DateTime CreatedTime { get; private set; }

    public CreateOrderLog(string orderId, DateTime createdTime)
    {
        OrderId = orderId;
        CreatedTime = createdTime;
    }
}