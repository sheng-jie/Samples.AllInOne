namespace CapDemo.Shared.Models;

public class DeduceInventoryDto
{
    public string OrderId { get; set; }

    public List<DeduceStockItem> DeduceStockItems { get; set; }
}

public class DeduceInventoryResult
{
    public string OrderId { get; set; }
    public bool IsSucceed { get; set; }

    public DeduceInventoryResult(string orderId, bool isSucceed)
    {
        OrderId = orderId;
        IsSucceed = isSucceed;
    }
}