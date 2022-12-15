using MassTransit.CourierDemo.InventoryService.Repositories;
using MassTransit.CourierDemo.Shared.Models;

namespace MassTransit.CourierDemo.InventoryService.Activities;

public class DeduceStockActivity : IActivity<DeduceOrderStockDto, DeduceStockLog>
{
    private readonly IRequestClient<IOrderItemsRequest> _orderItemsRequestClient;
    private readonly ILogger<DeduceStockActivity> _logger;

    public DeduceStockActivity(IRequestClient<IOrderItemsRequest> orderItemsRequestClient,
        ILogger<DeduceStockActivity> logger)
    {
        _orderItemsRequestClient = orderItemsRequestClient;
        _logger = logger;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<DeduceOrderStockDto> context)
    {
        var deduceStockDto = context.Arguments;
        var orderResponse =
            await _orderItemsRequestClient.GetResponse<IOrderItemsResponse>(new { deduceStockDto.OrderId });

        if (!CheckStock(orderResponse.Message.DeduceStockItems))
            return context.Faulted(new Exception("insufficient stock"));
        
        DeduceStocks(orderResponse.Message.DeduceStockItems);

        var log = new DeduceStockLog(deduceStockDto.OrderId, orderResponse.Message.DeduceStockItems);

        _logger.LogInformation($"Inventory has been deducted for order [{deduceStockDto.OrderId}]!");
        return context.CompletedWithVariables(log, new { log.OrderId });
    }

    private bool CheckStock(List<DeduceStockItem> deduceItems)
    {
        foreach (var stockItem in deduceItems)
        {
            if (InventoryRepository.GetStock(stockItem.SkuId) < stockItem.Qty) return false;
        }

        return true;
    }

    private void DeduceStocks(List<DeduceStockItem> deduceItems)
    {
        foreach (var stockItem in deduceItems)
        {
            InventoryRepository.TryDeduceStock(stockItem.SkuId, stockItem.Qty);
        }
    }

    public Task<CompensationResult> Compensate(CompensateContext<DeduceStockLog> context)
    {
        foreach (var deduceStockItem in context.Log.DeduceStockItems)
        {
            InventoryRepository.ReturnStock(deduceStockItem.SkuId, deduceStockItem.Qty);
        }

        _logger.LogWarning($"Inventory has been returned for order [{context.Log.OrderId}]!");
        return Task.FromResult(context.Compensated());
    }
}

public class DeduceStockLog
{
    public string OrderId { get; private set; }
    public DateTime CreatedTime { get; private set; }
    public List<DeduceStockItem> DeduceStockItems { get; private set; }

    public DeduceStockLog(string orderId, List<DeduceStockItem> deduceStockItems)
    {
        OrderId = orderId;
        CreatedTime = DateTime.Now;
        DeduceStockItems = deduceStockItems;
    }
}