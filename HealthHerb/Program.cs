using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthHerb.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HealthHerb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            Task.Run(async () =>
            {
                using var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
                var context = scope.ServiceProvider.GetService<AppDbContext>().Database;
                await context.MigrateAsync();
            }).Wait();

            host.Run();
        }

        public class AppDbFactory : IDesignTimeDbContextFactory<AppDbContext>
        {
            public AppDbContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("HealthHerb"));
                return new AppDbContext(optionsBuilder.Options);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
