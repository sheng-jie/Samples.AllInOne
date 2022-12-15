namespace MassTransit.CourierDemo.Shared.Models;

public interface IOrderItemsResponse
{
    public List<DeduceStockItem> DeduceStockItems { get; set; }
    public string OrderId { get; set; }
}