using System.Reflection;
using MassTransit.CourierDemo.Shared.Models;

namespace MassTransit.CourierDemo.PaymentService;

public static class MassTransitServiceExtensions
{
    public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
    {
        return services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            // By default, sagas are in-memory, but should be changed to a durable
            // saga repository.
            x.SetInMemorySagaRepositoryProvider();

            var entryAssembly = Assembly.GetEntryAssembly();

            x.AddConsumers(entryAssembly);
            x.AddSagaStateMachines(entryAssembly);
            x.AddSagas(entryAssembly);
            x.AddActivities(entryAssembly);
            x.AddRequestClient<IOrderAmountRequest>();
            x.UsingRabbitMq((context, busConfig) =>
            {
                busConfig.Host(
                    host: "localhost",
                    port: 5672,
                    virtualHost: "masstransit",
                    configure: hostConfig =>
                    {
                        hostConfig.Username("guest");
                        hostConfig.Password("guest");
                    });

                busConfig.ConfigureEndpoints(context);

                // busConfig.ReceiveEndpoint("orders",
                //     cfg => { cfg.ConfigureConsumer<OrderRequestConsumer>(context); });
                
                // busConfig.ReceiveEndpoint("orders",
                //     cfg =>
                //     {
                //         cfg.Handler<IOrderRequest>(async consumeContext =>
                //         {
                //             Console.WriteLine($"Receive order request:{consumeContext.Message.OrderId}");
                //             await consumeContext.RespondAsync<IOrderResponse>(new 
                //             {
                //                 Order = new Order()
                //                 {
                //                     OrderId = consumeContext.Message.OrderId,
                //                     Amount = DateTime.Now.Millisecond,
                //                     PaidTime = DateTime.Now.AddHours(-10)
                //                 }
                //             });
                //         });
                //     });
            });
        });
    }
}