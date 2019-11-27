using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Webapi.Demo
{
    /// <summary>
    /// 统一返回值
    /// </summary>
    public class UniformActionResult : IActionResult
    {
        public UniformActionResult(object data, int? statusCode = 200, Exception? exception = null)
        {
            Data = data;
            StatusCode = statusCode;
            Exception = exception;
        }

        public UniformActionResult()
        {
            
        }

        public UniformActionResult(UniformResult result)
        {
            StatusCode = result.StatusCode;
            Exception = result.Exception;
            Data = result.Data;
        }

        public Exception Exception { get; set; }

        public int? StatusCode { get; set; }

        public object Data { get; set; }

        public async Task ExecuteResultAsync(ActionContext context)
        {

            //var objectResult = new ObjectResult(this.Exception ?? this.Data)
            //{
            //    StatusCode = this.Exception != null
            //        ? StatusCodes.Status500InternalServerError
            //        : StatusCodes.Status200OK,
            //    ContentTypes = new MediaTypeCollection() { MediaTypeNames.Application.Json },
            //    DeclaredType = typeof(UniformResponse),
            //    Formatters = new FormatterCollection<IOutputFormatter>() { }
            //};

            var data = new
            {
                Code = this.StatusCode ?? this.Exception?.HResult,
                Data = Data,
                ErrorMsg = this.Exception?.Message
            };

            var jsonResult = new JsonResult(data);

            await jsonResult.ExecuteResultAsync(context);
        }
    }
}

