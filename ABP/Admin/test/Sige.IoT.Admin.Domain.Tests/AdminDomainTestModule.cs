using Sige.IoT.Admin.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Sige.IoT.Admin
{
    [DependsOn(
        typeof(AdminEntityFrameworkCoreTestModule)
        )]
    public class AdminDomainTestModule : AbpModule
    {

    }
}