using Microsoft.EntityFrameworkCore;

namespace DtmDemo.WebApi.Data
{
    public class DtmDemoWebApiContext : DbContext
    {
        public DtmDemoWebApiContext (DbContextOptions<DtmDemoWebApiContext> options)
            : base(options)
        {
        }

        public DbSet<DtmDemo.WebApi.Models.BankAccount> BankAccount { get; set; } = default!;
    }
}
