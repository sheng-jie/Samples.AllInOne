using MassTransit.Courier.Contracts;
using MassTransit.CourierDemo.Shared.Models;

namespace MassTransit.CourierDemo.OrderService;
public static class OrderRoutingSlipBuilder
{
    public static RoutingSlip BuildOrderRoutingSlip(CreateOrderDto createOrderDto)
    {
        var routingSlipBuilder = new RoutingSlipBuilder(Guid.NewGuid());
        var createOrderAddress = new Uri("queue:create-order_execute");
        var deduceStockAddress = new Uri("queue:deduce-stock_execute");
        var payAddress = new Uri("queue:pay-order_execute");

        routingSlipBuilder.AddActivity(
            name: "order-activity",
            executeAddress: createOrderAddress,
            arguments: createOrderDto);
        routingSlipBuilder.AddActivity(name: "deduce-stock-activity", executeAddress: deduceStockAddress);
        routingSlipBuilder.AddActivity(name: "pay-activity", executeAddress: payAddress);

        var routingSlip = routingSlipBuilder.Build();
        return routingSlip;
    }
}