using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestContainers.Container.Abstractions.Hosting;
using TestContainers.Container.Database.Hosting;
using TestContainers.Container.Database.PostgreSql;
using WebProShop.Data;
using WebProShop.Data.Migration;

namespace IntegrationTests
{
    public abstract class PostgreDbContextBase
    {
        private PostgreSqlContainer container;

        [OneTimeSetUp]
        public async Task Setup()
        {
            // Create a new PostgreSQL container with the specified username and password
            container = new ContainerBuilder<PostgreSqlContainer>()
                    .ConfigureDatabaseConfiguration("postgres", "postgres", "postgres")
                    .Build();

            await container.StartAsync();

            var migration = new PostgreSqlMigration(container.GetConnectionString(), Substitute.For<ILogger<PostgreSqlMigration>>());

            await migration.Migrate(default);
        }

        protected PgDataContext GetContext()
        {
            return new PgDataContext(container.GetConnectionString());
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            if (container != null)
            {
                await container.StopAsync();
            }
        }
    }
}
