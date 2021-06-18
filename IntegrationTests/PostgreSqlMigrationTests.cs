using NUnit.Framework;
using System.Threading.Tasks;
using TestContainers.Container.Abstractions.Hosting;
using TestContainers.Container.Database.PostgreSql;

using NSubstitute;
using Microsoft.Extensions.Logging;
using WebProShop.Data.Migration;
using System.Collections.Generic;

namespace IntegrationTests
{
    public class PostgreSqlMigrationTests
    {
        private PostgreSqlContainer container;
        private List<string> migrations;

        [OneTimeSetUp]
        public async Task Setup()
        {
            // Create a new PostgreSQL container with the specified username and password
            container = new ContainerBuilder<PostgreSqlContainer>()
                    .ConfigureDockerImageName("postgres:13.3-alpine")
                    .ConfigureContainer((h, p) => { p.Password = "postgres"; p.Username = "postgres"; })
                    .Build();

            await container.StartAsync();

            var migration = new PostgreSqlMigration(container.GetConnectionString(), Substitute.For<ILogger<PostgreSqlMigration>>());

            migrations = await migration.Migrate(default);
        }

        private async Task<Npgsql.NpgsqlConnection> GetConnectionAsync()
        {
            var result = new Npgsql.NpgsqlConnection(container.GetConnectionString());
            await result.OpenAsync();
            return result;
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            if (container != null)
            {
                await container.StopAsync();
            }
        }

        [Test]
        public void Migrate_MigrationsOk()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(migrations, Is.EqualTo(new string[] { "v1_0__Init.sql" }));
        }

        [TestCase("product")]
        [TestCase("shopping_cart")]
        [TestCase("shopping_cart_line")]
        public async Task Database_HasTable(string tableName)
        {
            // Arrange
            await using var connection = await GetConnectionAsync();

            // Act
            // Assert
            Assert.That(connection, Database.HasTable(tableName));
        }

        [TestCase("product", "id")]
        [TestCase("product", "name")]
        [TestCase("product", "description")]
        [TestCase("product", "price")]
        public async Task Table_HasColumn(string table, string column)
        {
            // Arrange
            await using var connection = await GetConnectionAsync();

            // Act
            // Assert
            Assert.That(connection, Database.HasTable(table).Column(column));
        }
    }
}