using MassTransit.SmDemo.InventoryService.Repositories;
using MassTransit.SmDemo.Shared.Contracts;

namespace MassTransit.SmDemo.InventoryService.Consumers;

public class DeduceInventoryConsumer : IConsumer<IDeduceInventoryCommand>
{
    private readonly ILogger<DeduceInventoryConsumer> _logger;

    public DeduceInventoryConsumer(ILogger<DeduceInventoryConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IDeduceInventoryCommand> context)
    {
        if (!CheckStock(context.Message.DeduceInventoryItems))
        {
            _logger.LogWarning($"Insufficient stock for order [{context.Message.OrderId}]!");
            await context.Publish<IDeduceInventoryFailed>(
                new { context.Message.OrderId, Reason = "insufficient stock" });
        }
        else
        {
            _logger.LogInformation($"Inventory has been deducted for order [{context.Message.OrderId}]!");
            DeduceStocks(context.Message.DeduceInventoryItems);
            await context.Publish<IDeduceInventorySucceed>(new { context.Message.OrderId });
        }
    }


    private bool CheckStock(List<DeduceInventoryItem> deduceItems)
    {
        foreach (var stockItem in deduceItems)
        {
            if (InventoryRepository.GetStock(stockItem.SkuId) < stockItem.Qty) return false;
        }

        return true;
    }

    private void DeduceStocks(List<DeduceInventoryItem> deduceItems)
    {
        foreach (var stockItem in deduceItems)
        {
            InventoryRepository.TryDeduceStock(stockItem.SkuId, stockItem.Qty);
        }
    }
}