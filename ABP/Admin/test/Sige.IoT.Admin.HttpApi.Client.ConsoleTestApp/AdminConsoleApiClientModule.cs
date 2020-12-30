using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace Sige.IoT.Admin.HttpApi.Client.ConsoleTestApp
{
    [DependsOn(
        typeof(AdminHttpApiClientModule),
        typeof(AbpHttpClientIdentityModelModule)
        )]
    public class AdminConsoleApiClientModule : AbpModule
    {
        
    }
}
