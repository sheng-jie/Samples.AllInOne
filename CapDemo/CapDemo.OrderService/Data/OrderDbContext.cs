using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CapDemo.OrderService.Domains;

namespace CapDemo.OrderService.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext (DbContextOptions<OrderDbContext> options)
            : base(options)
        {
        }

        public DbSet<CapDemo.OrderService.Domains.Order> Order { get; set; } = default!;
    }
}
