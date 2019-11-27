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
        private object _data;
        private int? _statusCode;
        private Exception _exception;

        public UniformActionResult(object data, int? statusCode = 200, Exception? exception = null)
        {
            Data = data;
            StatusCode = statusCode;
            Exception = exception;
        }

        public UniformActionResult(UniformResult result)
        {
            StatusCode = result.StatusCode;
            Exception = result.Exception;
            Data = result.Data;
        }

        public Exception Exception
        {
            get => _exception;
            set => _exception = value;
        }

        public int? StatusCode
        {
            get => _statusCode;
            set => _statusCode = value;
        }

        public object Data
        {
            get => _data;
            set => _data = value;
        }

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
                StatusCode = this.StatusCode ?? this.Exception?.HResult,
                Data = Data,
                ErrorMessage = this.Exception?.Message
            };

            var jsonResult = new JsonResult(data);

            await jsonResult.ExecuteResultAsync(context);
        }
    }
}

