namespace CapDemo.Shared.Models;

public class DeduceStockItem
{
    public string SkuId { get; private set; }

    public uint Qty { get; private set; }

    public decimal Price { get; set; }

    public DeduceStockItem(string skuId, uint qty, decimal price)
    {
        SkuId = skuId;
        Qty = qty;
        Price = price;
    }
}