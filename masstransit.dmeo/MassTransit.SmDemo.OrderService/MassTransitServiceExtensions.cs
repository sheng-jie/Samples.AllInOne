﻿using System.Reflection;
using MassTransit.SmDemo.OrderService.Consumer;
using MassTransit.SmDemo.OrderService.StateMachines;
using MassTransit.SmDemo.Share.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace MassTransit.SmDemo.OrderService;

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
            
            x.AddSagaStateMachine<OrderStateMachine, OrderState>(typeof(OrderStateMachineDefinition))
                .InMemoryRepository();
            
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
                
                busConfig.ReceiveEndpoint("cancel-order",
                    cfg => { cfg.ConfigureConsumer<CancelOrderRequestConsumer>(context); });
                busConfig.ConfigureEndpoints(context);
            });
            
            x.AddRequestClient<IReserveStockRequest>();
        });
    }
}