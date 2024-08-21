using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EvolveDb;
using Microsoft.Extensions.Logging;

namespace WebProShop.Data.Migration;

public sealed class PostgreSqlMigration(string connectionString, ILogger<PostgreSqlMigration> logger)
    : IDatabaseMigration
{
    private readonly string connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

    public async Task<List<string>> Migrate(CancellationToken cancel)
    {
        await using var conn = new Npgsql.NpgsqlConnection(connectionString);
        await conn.OpenAsync(cancel);

        var ev = new Evolve(conn, str => logger.LogInformation(str))
        {
            EmbeddedResourceAssemblies = new[] { typeof(PostgreSqlMigration).Assembly },
            EmbeddedResourceFilters = new[] { "WebProShop.Data.Migration.PostgreSql" }
        };

        await Task.Run(ev.Migrate, cancel);

        await conn.ReloadTypesAsync();

        return ev.AppliedMigrations;
    }
}
