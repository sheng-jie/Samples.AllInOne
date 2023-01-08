using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DotNetCore.Cap.WebApiDemo;

namespace DotNetCore.Cap.WebApiDemo.Data
{
    public class WeatherDbContext : DbContext
    {
        public WeatherDbContext (DbContextOptions<WeatherDbContext> options)
            : base(options)
        {
        }

        public DbSet<DotNetCore.Cap.WebApiDemo.WeatherForecast> WeatherForecast { get; set; } = default!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
