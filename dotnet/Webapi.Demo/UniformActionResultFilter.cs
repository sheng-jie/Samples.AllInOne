using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Webapi.Demo
{
    public class UniformActionResultFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is UniformActionResult)
            {
                await next();
            }

            var uniformResult = new UniformActionResult();

            if (context.Result is ObjectResult objectResult)
            {
                uniformResult.Data = objectResult.StatusCode == StatusCodes.Status200OK ? objectResult.Value : null;
                uniformResult.StatusCode = objectResult.StatusCode;
            }


            await uniformResult.ExecuteResultAsync(context);

            context.Result = uniformResult;



        }
    }
}