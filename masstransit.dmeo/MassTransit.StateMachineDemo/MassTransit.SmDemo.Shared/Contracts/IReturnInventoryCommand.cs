namespace MassTransit.SmDemo.Shared.Contracts;

public interface IReturnInventoryCommand
{
    public Guid OrderId { get; set; }
    public List<ReturnInventoryItem> ReturnInventoryItems { get; set; }
}

public class ReturnInventoryItem : InventoryItem
{
    public ReturnInventoryItem(string skuId, uint qty) : base(skuId, qty)
    {
    }
}

public interface IReturnInventorySucceed
{
    public Guid OrderId { get; set; }
}