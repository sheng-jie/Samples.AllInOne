using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Hosting;

namespace Ocelot.Api1
{
    public static class ConsulExtensions
    {
        public static IApplicationBuilder RegisterConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            var consulClient = new ConsulClient(x => x.Address = new Uri("http://localhost:8500"));

            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features?.Get<IServerAddressesFeature>();
            var address = addresses?.Addresses.First();
            var uri = new Uri(address);

            var healthCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                Interval = TimeSpan.FromSeconds(10),
                HTTP = $"{uri.AbsoluteUri}health",
                Timeout = TimeSpan.FromSeconds(5)
            };
;

            
            var registration = new AgentServiceRegistration()
            {
                Check = healthCheck,
                ID = "Ocelot.Api1",
                Name = "Ocelot.Api1",
                Address = $"{uri.Scheme}://{uri.Host}",
                Port = uri.Port,
                Tags = new[] { "Ocelot.Api1" }

            };

            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            consulClient.Agent.ServiceRegister(registration).Wait();

            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });

            return app;
        }
    }
}
