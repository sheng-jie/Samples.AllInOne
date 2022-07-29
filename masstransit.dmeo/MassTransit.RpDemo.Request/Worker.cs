using MassTransit.RequestResponseDemo;
using Microsoft.Extensions.Hosting;

namespace MassTransit.RpDemo.Request;

public class Worker : BackgroundService
{
    readonly IBus _bus;

    public Worker(IBus bus)
    {
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var serviceAddress = "rabbitmq://localhost:5672/orders";
        var orderRequestClient = _bus.CreateRequestClient<IOrderRequest>(new Uri(serviceAddress), TimeSpan.FromSeconds(60));
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var orderId = Guid.NewGuid().ToString();
            var requestTask =  orderRequestClient.GetResponse<IOrderResponse>(new OrderRequest(orderId));

            var response = await requestTask;

            Console.WriteLine(
                $"Get order {orderId} succeed:{response.Message.Order.Amount},{response.Message.Order.PaidTime}");

            await Task.Delay(1000, stoppingToken);
        }
    }
}
// {
// "messageId": "ec0e0000-873b-00ff-630e-08da71b84291",
// "requestId": "ec0e0000-873b-00ff-f368-08da71b8424e",
// "correlationId": null,
// "conversationId": "ec0e0000-873b-00ff-fd58-08da71b84298",
// "initiatorId": null,
// "sourceAddress": "rabbitmq://localhost/masstransit/THINKPAD_MassTransitRpDemoRequest_bus_7o8yyyr88cyx9nznbdp8dqnnbe?temporary=true",
// "destinationAddress": "rabbitmq://localhost/masstransit/orders?bind=true",
// "responseAddress": "rabbitmq://localhost/masstransit/THINKPAD_MassTransitRpDemoRequest_bus_7o8yyyr88cyx9nznbdp8dqnnbe?temporary=true",
// "faultAddress": null,
// "messageType": [
// "urn:message:MassTransit.RequestResponseDemo:IOrderRequest"
//     ],
// "message": {
//     "orderId": "5ca13bde-8c17-4753-903a-a445389b98e5"
// },
// "expirationTime": "2022-07-29T23:16:13.3233807Z",
// "sentTime": "2022-07-29T23:15:43.1091982Z",
// "headers": {
//     "MT-Request-AcceptType": [
//     "urn:message:MassTransit.RequestResponseDemo:IOrderResponse"
//         ]
// },
// "host": {
//     "machineName": "THINKPAD",
//     "processName": "MassTransit.RpDemo.Request",
//     "processId": 3820,
//     "assembly": "MassTransit.RpDemo.Request",
//     "assemblyVersion": "1.0.0.0",
//     "frameworkVersion": "6.0.5",
//     "massTransitVersion": "8.0.5.0",
//     "operatingSystemVersion": "Microsoft Windows NT 10.0.19044.0"
// }
// }