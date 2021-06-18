using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebProShop.Data.Migration;

namespace WebProShop
{
    public class Program
    {
        public static Task Main(string[] args) => MainAsync(args, default);

        public static async Task MainAsync(string[] args, System.Threading.CancellationToken cancellationToken)
        {
            var host = CreateHostBuilder(args).Build();
            var migration = host.Services.GetService<IDatabaseMigration>();
            await migration.Migrate(cancellationToken);
            await host.RunAsync(cancellationToken);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
