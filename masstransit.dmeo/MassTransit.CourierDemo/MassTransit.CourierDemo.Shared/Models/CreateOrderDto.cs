namespace MassTransit.CourierDemo.Shared.Models;

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