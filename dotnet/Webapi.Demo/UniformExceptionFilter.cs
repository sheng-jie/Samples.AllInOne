using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;

namespace Webapi.Demo
{
    public class UniformExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogger<UniformExceptionFilter> _logger;

        public UniformExceptionFilter(ILogger<UniformExceptionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            var result = new UniformActionResult()
            {
                Exception = context.Exception
            };

            await result.ExecuteResultAsync(context);

            _logger.LogError(context.Exception,context.Exception.Message);

            context.ExceptionHandled = true;
            context.Result = result;
        }
    }
}