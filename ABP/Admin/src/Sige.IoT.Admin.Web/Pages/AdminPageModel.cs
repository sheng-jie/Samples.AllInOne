using Sige.IoT.Admin.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Sige.IoT.Admin.Web.Pages
{
    /* Inherit your PageModel classes from this class.
     */
    public abstract class AdminPageModel : AbpPageModel
    {
        protected AdminPageModel()
        {
            LocalizationResourceType = typeof(AdminResource);
        }
    }
}