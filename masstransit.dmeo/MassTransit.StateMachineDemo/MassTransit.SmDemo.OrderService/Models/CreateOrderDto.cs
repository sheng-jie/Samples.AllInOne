namespace MassTransit.SmDemo.OrderService.Models;

public class CreateOrderDto
{
    public string CustomerId { get; private set; }
    public List<ShoppingCartItemDto> ShoppingCartItems { get; private set; }

    public CreateOrderDto(string customerId, List<ShoppingCartItemDto> shoppingCartItems)
    {
        CustomerId = customerId;
        ShoppingCartItems = shoppingCartItems;
    }

}

public class ShoppingCartItemDto
{
    public string SkuId { get; set; }
    public decimal Price { get; set; }

    public uint Qty { get; set; }
}