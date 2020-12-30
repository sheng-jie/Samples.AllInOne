using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;

namespace Dotnet.Diagnostic.Demo
{
    public class DiagnosticObserver : IObserver<KeyValuePair<string, object>>
    {
        public void OnCompleted()
        {
            //Noting to do
        }

        public void OnError(Exception error)
        {
            Console.WriteLine($"{error.Message}");
        }

        private ConcurrentDictionary<string, long> startTimes = new ConcurrentDictionary<string, long>();

        public void OnNext(KeyValuePair<string, object> pair)
        {
            Console.WriteLine($"{pair.Key}-{pair.Value}");

            //获取httpContext
            var context = pair.Value.GetType().GetTypeInfo().GetDeclaredProperty("httpContext")
                ?.GetValue(pair.Value) as DefaultHttpContext;
            //获取timestamp
            var timestamp = pair.Value.GetType().GetTypeInfo().GetDeclaredProperty("timestamp")
                ?.GetValue(pair.Value) as long?;

            switch (pair.Key)
            {
                case "Microsoft.AspNetCore.Hosting.BeginRequest":
                    Console.WriteLine($"Request {context.TraceIdentifier} Begin:{context.Request.GetUri()}");
                    startTimes.TryAdd(context.TraceIdentifier, timestamp.Value);//记录请求开始时间
                    break;
                case "Microsoft.AspNetCore.Hosting.EndRequest":
                    startTimes.TryGetValue(context.TraceIdentifier, out long startTime);
                    var elapsedMs = (timestamp - startTime) / TimeSpan.TicksPerMillisecond;//计算耗时
                    Console.WriteLine(
                        $"Request {context.TraceIdentifier} End: Status Code is {context.Response.StatusCode},Elapsed {elapsedMs}ms");
                    startTimes.TryRemove(context.TraceIdentifier, out _);
                    break;
            }

            // switch (value.Key)
            // {
            //     case "Dotnet.Diagnostic.Demo.LogHttpContextMiddleware.Start":
            //         Console.WriteLine("Start");
            //         break;
            //     case "Dotnet.Diagnostic.Demo.LogHttpContextMiddleware.End":
            //         Console.WriteLine("End");
            //         break;
            //     case "Microsoft.AspNetCore.Hosting.HttpRequestIn.Start":
            //         Console.WriteLine("Http request entered!");
            //         break;
            //     case "Activity.Start":
            //         Console.WriteLine($"Activity.Start - activity id: {Activity.Current?.Id}");
            //         break;
            //     case "Activity.Stop":
            //         Console.WriteLine("Activity.Stop");
            //
            //         if (Activity.Current != null)
            //         {
            //             foreach (var tag in Activity.Current.Tags)
            //             {
            //                 Console.WriteLine($"{tag.Key} - {tag.Value}");
            //             }
            //         }
            //
            //         break;
            //     default:
            //         break;
            // }
        }
    }
}