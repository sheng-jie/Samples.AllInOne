using MassTransit.SmDemo.Share.Contracts;

namespace MassTransit.SmDemo.StockService.Consumers;

public class ReserveStockRequestConsumer : IConsumer<IReserveStockRequest>
{
    public async Task Consume(ConsumeContext<IReserveStockRequest> context)
    {
        var isSucceed = GoodStockStore.ReserveStock(context.Message.Items);

        await context.RespondAsync<IReserveStockResponse>(new
        {
            IsSucceed = isSucceed,
            Message = isSucceed ? "库存预留成功！" : "库存不足！"
        });
    }
}

public class CheckStockRequestConsumer : IConsumer<ICheckStockRequest>
{
    public async Task Consume(ConsumeContext<ICheckStockRequest> context)
    {
        var stocks = GoodStockStore.GetStocks();

        await context.RespondAsync<ICheckStockResponse>(new 
        {
            Stocks = stocks
        });
    }
    
    
}