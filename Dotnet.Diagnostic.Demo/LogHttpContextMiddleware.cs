using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Dotnet.Diagnostic.Demo
{
    public class LogHttpContextMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly DiagnosticSource _diagnosticSource;

        private static readonly string logHttpContextDiagnosticSourceCode = typeof(LogHttpContextMiddleware).FullName;

        public LogHttpContextMiddleware(RequestDelegate next,DiagnosticSource diagnosticSource)
        {
            _next = next;
            _diagnosticSource = diagnosticSource;
        }

        public async Task Invoke(HttpContext context)
        {
            if (_diagnosticSource.IsEnabled($"{logHttpContextDiagnosticSourceCode}.Start"))
            {
                _diagnosticSource.Write($"{logHttpContextDiagnosticSourceCode}.Start",new {HttpContext=context});
            }

            await _next.Invoke(context);
            
            if (_diagnosticSource.IsEnabled($"{logHttpContextDiagnosticSourceCode}.End"))
            {
                _diagnosticSource.Write($"{logHttpContextDiagnosticSourceCode}.End",new {Body =context.Response.Body.CanRead});
            }
        }
    }
}