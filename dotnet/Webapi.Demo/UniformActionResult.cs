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
        private readonly object _data;
        private readonly int? _statusCode;
        private readonly Exception _exception;

        public UniformActionResult(object data, int? statusCode = 200, Exception? exception = null)
        {
            _data = data;
            _statusCode = statusCode;
            _exception = exception;
        }

        public UniformActionResult(UniformResult result)
        {
            _statusCode = result.StatusCode;
            _exception = result.Exception;
            _data = result.Data;
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
                StatusCode = this._statusCode ?? this._exception?.HResult,
                Data = _data,
                ErrorMessage = this._exception?.Message
            };

            var jsonResult = new JsonResult(data);

            await jsonResult.ExecuteResultAsync(context);
        }
    }
}

