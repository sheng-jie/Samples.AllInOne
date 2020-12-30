using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Loki.K8s.Demo
{
    public class Startup
    {

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ScopedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    logger.LogInformation("start handle request!");
                    var factory = context.RequestServices.GetRequiredService<IServiceScopeFactory>();
                    using (var scope = factory.CreateScope())
                    {
                        var scopedSvc = scope.ServiceProvider.GetRequiredService<ScopedService>();
                        var str = scopedSvc.GetStr();
                        await context.Response.WriteAsync($"Hello World!{str}");
                        //scopedSvc.Dispose();
                    }
                    logger.LogInformation("end handle request!");
                });
            });
        }
    }

    public class ScopedService : IDisposable
    {
        private readonly ILogger<ScopedService> logger;

        public ScopedService(ILogger<ScopedService> logger)
        {
            logger.LogInformation($"{this.GetType()} has initialized!");
            this.logger = logger;
        }
        public void Dispose()
        {
            logger.LogInformation($"{this.GetType()} is disposed!");
        }

        public string GetStr()
        {

            //throw new ArgumentNullException();
            return Guid.NewGuid().ToString();
        }
    }
}
