using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DiagnosticAdapter;

namespace Dotnet.Diagnostic.Demo
{
    public class HttpContextDiagnosticListener
    {
        [DiagnosticName("Dotnet.Diagnostic.Demo.LogHttpContextMiddleware.Start")]
        public virtual void Start(HttpContext httpContext)
        {
            Console.WriteLine($"Demo Middleware Starting, request path: {httpContext.Request.Path}");
        }
        
        [DiagnosticName("Dotnet.Diagnostic.Demo.LogHttpContextMiddleware.End")]
        public virtual void End()
        {
            Console.WriteLine($"Demo Middleware End");
        }
    }
}