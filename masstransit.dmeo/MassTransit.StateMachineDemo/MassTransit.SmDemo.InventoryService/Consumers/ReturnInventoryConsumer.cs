using MassTransit.SmDemo.InventoryService.Repositories;
using MassTransit.SmDemo.Shared.Contracts;

namespace MassTransit.SmDemo.InventoryService.Consumers;

public class ReturnInventoryConsumer : IConsumer<IReturnInventoryCommand>
{
    private readonly ILogger<ReturnInventoryConsumer> _logger;

    public ReturnInventoryConsumer(ILogger<ReturnInventoryConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IReturnInventoryCommand> context)
    {
        foreach (var returnInventoryItem in context.Message.ReturnInventoryItems)
        {
            InventoryRepository.ReturnStock(returnInventoryItem.SkuId, returnInventoryItem.Qty);
        }

        _logger.LogInformation($"Inventory has been returned for order [{context.Message.OrderId}]!");
        await context.Publish<IReturnInventorySucceed>(new { context.Message.OrderId });
    }
}