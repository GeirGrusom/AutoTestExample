using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using WebProShop.Data.Migration;

namespace WebProShop;

public static class Program
{
    public static Task Main(string[] args) => MainAsync(args, default);

    public static async Task MainAsync(string[] args, System.Threading.CancellationToken cancellationToken)
    {
            var host = CreateHostBuilder(args).Build();
            var migration = host.Services.GetService<IDatabaseMigration>();
            await migration.Migrate(cancellationToken);
            await host.RunAsync(cancellationToken);
        }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
