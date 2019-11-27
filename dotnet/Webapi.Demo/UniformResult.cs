using System;

namespace Webapi.Demo
{
    public class UniformResult
    {
        public object Data { get; set; }
        public int? StatusCode { get; set; }
        public Exception Exception { get; set; }

        public string ErrMsg { get; set; }

        public UniformResult(object data, int? statusCode = 200, string errMsg = null)
        {
            Data = data;
            StatusCode = statusCode;
            ErrMsg = errMsg;
        }

        

        public UniformResult(object data, int? statusCode = 200, Exception? exception = null)
        {
            Data = data;
            StatusCode = statusCode;
            Exception = exception;
        }
    }
}