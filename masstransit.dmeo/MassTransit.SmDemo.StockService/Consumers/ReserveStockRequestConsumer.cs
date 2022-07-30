using MassTransit.SmDemo.Share.Contracts;

namespace MassTransit.SmDemo.StockService.Consumers;

public class CheckStockRequestConsumer : IConsumer<IReserveStockRequest>
{
    public async Task Consume(ConsumeContext<IReserveStockRequest> context)
    {
        var hasStock = CheckStock(context.Message);

        await context.RespondAsync<IReserveStockResponse>(new
        {
            IsSucceed = hasStock,
            Message = hasStock ? "库存预留成功！" : "库存不足！"
        });
    }

    private bool CheckStock(IReserveStockRequest request)
    {
        var hasStock = request.Items.All(item => GoodStockStore.HasStock(item.Key, item.Value));
        return hasStock;
    }
}