using Volo.Abp.Modularity;

namespace Sige.IoT.Admin
{
    [DependsOn(
        typeof(AdminApplicationModule),
        typeof(AdminDomainTestModule)
        )]
    public class AdminApplicationTestModule : AbpModule
    {

    }
}