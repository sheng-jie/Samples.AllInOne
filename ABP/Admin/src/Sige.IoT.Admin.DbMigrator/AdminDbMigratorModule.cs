using Sige.IoT.Admin.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace Sige.IoT.Admin.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AdminEntityFrameworkCoreDbMigrationsModule),
        typeof(AdminApplicationContractsModule)
        )]
    public class AdminDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<BackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
