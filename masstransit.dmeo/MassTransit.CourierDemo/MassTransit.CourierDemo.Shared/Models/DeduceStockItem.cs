namespace MassTransit.CourierDemo.Shared.Models;

public class DeduceStockItem
{
    public string SkuId { get; private set; }

    public uint Qty { get; private set; }

    public DeduceStockItem(string skuId, uint qty)
    {
        SkuId = skuId;
        Qty = qty;
    }
}