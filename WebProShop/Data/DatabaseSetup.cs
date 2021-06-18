using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebProShop.Data.Migration;
using WebProShop.Data.Models;

namespace WebProShop.Data
{
    public static class DatabaseSetup
    {
        public static IServiceCollection AddPostgreSql(this IServiceCollection services, string connectionString)
        {
            return services
                .AddScoped<DbContext>(_ => new PgDataContext(connectionString))
                .AddTransient<IDatabaseMigration>(sp => new PostgreSqlMigration(connectionString, sp.GetRequiredService<ILogger<PostgreSqlMigration>>()))
                .AddTransient(sp => ((PgDataContext)sp.GetRequiredService<DbContext>()).CreateUnitOfWork());
            
        }

        public static IServiceCollection AddQueryables(this IServiceCollection services)
        {
            return services
                .AddQueryable<Product>()
                .AddQueryable<ShoppingCart>();
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQueryable<T>(this IServiceCollection services)
            where T : class
        {
            return services.AddScoped<IQueryable<T>>(sp => sp.GetRequiredService<DbContext>().Set<T>());
        }
    }

}
