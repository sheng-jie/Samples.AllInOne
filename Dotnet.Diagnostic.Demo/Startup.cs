using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dotnet.Diagnostic.Demo.SubscriberPattern;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DiagnosticAdapter;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dotnet.Diagnostic.Demo
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DiagnosticListener diagnosticListener)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            // diagnosticListener.Subscribe(new DiagnosticObserver());
            diagnosticListener.SubscribeWithAdapter(new HttpContextDiagnosticObserver());
           
            app.UseStatusCodePages();
            app.UseWelcomePage("/welcome");
            // app.UseMiddleware<LogHttpContextMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/subscriber", async context =>
                {
                    var kettle = new Kettle();
                    var subscribeRef = kettle.Subscribe(new Alter());
                    
                    var boilTask = kettle.StartBoilWaterAsync();
                    var timoutTask = Task.Delay(TimeSpan.FromSeconds(15));

                    var firstReturnTask = await Task.WhenAny(boilTask, timoutTask);
                    //如果15s未烧开，则取消订阅
                    if (firstReturnTask == timoutTask)
                        subscribeRef.Dispose();

                    await context.Response.WriteAsync("Hello subscriber!");
                });
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapGet("/activity", async context =>
                {
                    var activity = new Activity("Activity");
                    var id = new Random().Next();
                    if (diagnosticListener.IsEnabled("Activity.Start"))
                    {
                        diagnosticListener.StartActivity(activity, new { IdArg =  id});
                    }

                    activity.AddTag("MyTagId", "ValueInTags");
                    activity.AddBaggage("MyBaggageId", "ValueInBaggage");

                    var httpClient = new HttpClient();
                    await httpClient.GetAsync("http://localhost:39231/");

                    if (diagnosticListener.IsEnabled("Activity.Stop"))
                    {
                        diagnosticListener.StopActivity(activity, new { IdArg = id });
                    }
                    
                    
                    await context.Response.WriteAsync("Hello Activity!");
                });
            });
        }
    }
}