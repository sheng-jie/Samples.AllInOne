using Microsoft.AspNetCore.Mvc;

namespace Webapi.Demo
{
    /// <summary>
    /// 所有控制器的基类
    /// </summary>
    [ServiceFilter(typeof(UniformActionResultFilter))]
    public class UniformBaseController : ControllerBase
    {
        [NonAction]
        public virtual IActionResult NotOk(int? statusCode = 500, string errorMsg = "")
        {
            return new UniformActionResult()
            {
                StatusCode = statusCode,
                ErrMsg = errorMsg
            };
        }
    }
}