using MassTransit.SmDemo.OrderService.Models;

namespace MassTransit.SmDemo.OrderService.Events;

public interface ICreateOrderCommand
{
    public string CustomerId { get;  set; }
    public List<ShoppingCartItemDto> ShoppingCartItems { get; set; }
}