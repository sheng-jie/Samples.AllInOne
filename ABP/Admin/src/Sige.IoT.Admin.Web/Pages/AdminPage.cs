using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Sige.IoT.Admin.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Sige.IoT.Admin.Web.Pages
{
    /* Inherit your UI Pages from this class. To do that, add this line to your Pages (.cshtml files under the Page folder):
     * @inherits Sige.IoT.Admin.Web.Pages.AdminPage
     */
    public abstract class AdminPage : AbpPage
    {
        [RazorInject]
        public IHtmlLocalizer<AdminResource> L { get; set; }
    }
}
