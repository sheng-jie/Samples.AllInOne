using MassTransit;
using MassTransitDemo.MediatorService.Consumers;

var mediator = Bus.Factory.CreateMediator(cfg =>
{
    cfg.Consumer<SubmitOrderConsumer>();
    cfg.Consumer<OrderStatusConsumer>();
});

var orderId = NewId.NextGuid();

await mediator.Send<SubmitOrder>(new { ProductId = orderId });

var client = mediator.CreateRequestClient<GetOrderStatus>();
var response = await client.GetResponse<OrderStatus>(new { OrderId = orderId });
Console.WriteLine("Order Status: {0}", response.Message.Status);