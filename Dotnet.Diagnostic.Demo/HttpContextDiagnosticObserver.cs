using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DiagnosticAdapter;

namespace Dotnet.Diagnostic.Demo
{
    public sealed class HttpContextDiagnosticObserver
    {
        private ConcurrentDictionary<string, long> startTimes = new ConcurrentDictionary<string, long>();
        
        [DiagnosticName("Microsoft.AspNetCore.Hosting.BeginRequest")]
        public void BeginRequest(HttpContext httpContext,long timestamp)
        {
            Console.WriteLine($"Request {httpContext.TraceIdentifier} {Activity.Current.Id} Begin:{httpContext.Request.GetUri()}");
            startTimes.TryAdd(httpContext.TraceIdentifier, timestamp);//记录请求开始时间
        }
        
        [DiagnosticName("Microsoft.AspNetCore.Hosting.EndRequest")]
        public void EndRequest(HttpContext httpContext,long timestamp)
        {
            startTimes.TryGetValue(httpContext.TraceIdentifier, out long startTime);
            var elapsedMs = (timestamp - startTime) / TimeSpan.TicksPerMillisecond;//计算耗时
            Console.WriteLine(
                $"Request {httpContext.TraceIdentifier} {Activity.Current.Id} End: Status Code is {httpContext.Response.StatusCode},Elapsed {elapsedMs}ms");
            startTimes.TryRemove(httpContext.TraceIdentifier, out _);
        }
        
        [DiagnosticName("Activity.Start")]
        public void ActivityStart(int idArg)
        {
        }
        [DiagnosticName("Activity.Stop")]
        public void ActivityStop(int idArg)
        {
        }
    }
}