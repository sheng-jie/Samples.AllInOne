using System;
using System.Collections.Generic;
using System.Text;
using Sige.IoT.Admin.Localization;
using Volo.Abp.Application.Services;

namespace Sige.IoT.Admin
{
    /* Inherit your application services from this class.
     */
    public abstract class AdminAppService : ApplicationService
    {
        protected AdminAppService()
        {
            LocalizationResource = typeof(AdminResource);
        }
    }
}
