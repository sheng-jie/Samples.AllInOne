using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Webapi.Demo
{
    /// <summary>
    /// 统一Action Filter
    /// </summary>
    public class UniformActionFilter : IAsyncActionFilter, IActionFilter
    {
        private readonly ILogger<UniformActionFilter> _logger;

        readonly Stopwatch _watch = new Stopwatch();

        public UniformActionFilter(ILogger<UniformActionFilter> logger)
        {
            _logger = logger;
        }

        public  async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _watch.Start();
            if (context.ModelState.IsValid)
            {
                await next();
            }
            else
            {context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _watch.Stop();

            _logger.LogInformation($"It takes {_watch.ElapsedMilliseconds}ms");


        }
    }
}