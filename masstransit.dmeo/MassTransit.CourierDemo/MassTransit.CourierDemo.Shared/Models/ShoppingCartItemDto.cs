namespace MassTransit.CourierDemo.Shared.Models;

public class ShoppingCartItemDto
{
    public string SkuId { get; set; }
    public decimal Price { get; set; }

    public uint Qty { get; set; }
}