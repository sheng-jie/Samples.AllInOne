using Sige.IoT.Admin.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Sige.IoT.Admin.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class AdminController : AbpController
    {
        protected AdminController()
        {
            LocalizationResource = typeof(AdminResource);
        }
    }
}