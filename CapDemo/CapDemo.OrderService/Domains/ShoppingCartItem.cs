namespace CapDemo.OrderService.Domains;

public class ShoppingCartItem
{
    public string SkuId { get; }
    public decimal Price { get;}
    public uint Qty { get; }

    public ShoppingCartItem(string skuId, decimal price, uint qty)
    {
        SkuId = skuId;
        Price = price;
        Qty = qty;
    }
}