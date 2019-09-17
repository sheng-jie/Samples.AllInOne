using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sige.IoT.Admin.Data;
using Volo.Abp.DependencyInjection;

namespace Sige.IoT.Admin.EntityFrameworkCore
{
    [Dependency(ReplaceServices = true)]
    public class EntityFrameworkCoreAdminDbSchemaMigrator 
        : IAdminDbSchemaMigrator, ITransientDependency
    {
        private readonly AdminMigrationsDbContext _dbContext;

        public EntityFrameworkCoreAdminDbSchemaMigrator(AdminMigrationsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task MigrateAsync()
        {
            await _dbContext.Database.MigrateAsync();
        }
    }
}