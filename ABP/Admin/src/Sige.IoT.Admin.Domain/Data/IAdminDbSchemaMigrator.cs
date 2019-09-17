using System.Threading.Tasks;

namespace Sige.IoT.Admin.Data
{
    public interface IAdminDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
