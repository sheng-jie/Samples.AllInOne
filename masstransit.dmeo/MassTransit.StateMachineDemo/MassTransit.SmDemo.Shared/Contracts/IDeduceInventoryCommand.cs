namespace MassTransit.SmDemo.Shared.Contracts;

public interface IDeduceInventoryCommand
{
    public Guid OrderId { get; set; }
    public List<DeduceInventoryItem> DeduceInventoryItems { get; set; }
}

public class InventoryItem
{
    public string SkuId { get; private set; }

    public uint Qty { get; private set; }

    public InventoryItem(string skuId, uint qty)
    {
        SkuId = skuId;
        Qty = qty;
    }
}



public class DeduceInventoryItem : InventoryItem
{
    public DeduceInventoryItem(string skuId, uint qty) : base(skuId, qty)
    {
    }
}