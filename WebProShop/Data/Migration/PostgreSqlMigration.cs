using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Evolve;
using Microsoft.Extensions.Logging;

namespace WebProShop.Data.Migration
{
    public sealed class PostgreSqlMigration : IDatabaseMigration
    {
        private readonly string connectionString;
        private readonly ILogger<PostgreSqlMigration> logger;

        public PostgreSqlMigration(string connectionString, ILogger<PostgreSqlMigration> logger)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            this.logger = logger;
        }

        public async Task<List<string>> Migrate(CancellationToken cancel)
        {
            using var conn = new Npgsql.NpgsqlConnection(connectionString);
            await conn.OpenAsync();

            var ev = new Evolve.Evolve(conn, str => this.logger.LogInformation(str))
            {
                EmbeddedResourceAssemblies = new[] { typeof(PostgreSqlMigration).Assembly },
                EmbeddedResourceFilters = new[] { "WebProShop.Data.Migration.PostgreSql" }
            };

            await Task.Run(ev.Migrate, cancel);

            conn.ReloadTypes();

            return ev.AppliedMigrations;
        }
    }
}
