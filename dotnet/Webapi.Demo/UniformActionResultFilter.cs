using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Webapi.Demo
{
    public class UniformActionResultFilter:IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (!(context.Result is UniformActionResult))
            {
                var uniformResult =  new UniformActionResult(context.Result);

                await uniformResult.ExecuteResultAsync(context);

                context.Result = uniformResult;

            }

            await next();
        }
    }
}