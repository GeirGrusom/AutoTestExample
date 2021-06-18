using NUnit.Framework;
using RestSharp.Serializers.SystemTextJson;
using System;
using System.Threading.Tasks;
using TestContainers.Container.Abstractions.Hosting;
using TestContainers.Container.Database.PostgreSql;
using WebProShop;
using WebProShop.Controllers;
using WebProShop.Models;

namespace SystemTests
{
    [NUnit.Framework.NonParallelizable]
    //[Category("SkipWhenLiveUnitTesting")]
    public class ProductsControllerTests
    {
        PostgreSqlContainer databaseContainer;
        System.Threading.CancellationTokenSource cancel = new System.Threading.CancellationTokenSource();
        Task appTask;

        private static RestSharp.IRestClient CreateClient()
        {
            return new RestSharp.RestClient("http://localhost:8088/").UseSystemTextJson(options);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            databaseContainer = new ContainerBuilder<PostgreSqlContainer>()
            .ConfigureDockerImageName("postgres:13.3-alpine")
            .ConfigureContainer((h, p) => { p.Password = "postgres"; p.Username = "postgres"; })
            .Build();

            await databaseContainer.StartAsync();
            
            appTask = Task.Run(() => Program.MainAsync(new[] { "--urls=http://*:8088/", "--environment=Development", "--ConnectionStrings:PostgreSql", databaseContainer.GetConnectionString() } , cancel.Token));

            await WaitForHealthCheck();
            
        }

        private async Task WaitForHealthCheck()
        {
            var client = CreateClient();
            var healthCheck = new RestSharp.RestRequest("/", RestSharp.Method.GET);
            while (true)
            {
                var response = await client.ExecuteAsync(healthCheck, cancel.Token);
                if (response.IsSuccessful)
                {
                    return;
                }
            }
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            cancel.Cancel();
            try
            {
                await appTask;
            }
            catch(OperationCanceledException)
            {

            }

            if(databaseContainer is not null)
            {
                await databaseContainer.StopAsync();
            }
        }

        private static readonly System.Text.Json.JsonSerializerOptions options = new(System.Text.Json.JsonSerializerDefaults.Web);

        [Test]
        public async Task Get_ReturnsPagedResult()
        {
            // Arrange
            var client = CreateClient();

            // Act
            var result = await client.ExecuteAsync<PagedResult<ProductResult>>(new RestSharp.RestRequest("/products", RestSharp.Method.GET));

            // Assert
            Assert.That(result.Data, Is.InstanceOf<PagedResult<ProductResult>>());
        }

        [Test]
        public async Task Get_ProductDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var client = CreateClient();
            var request = new RestSharp.RestRequest($"/products/{Guid.NewGuid():N}", RestSharp.Method.GET);

            // Act
            var result = await client.ExecuteAsync<PagedResult<ProductResult>>(request);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
        }
    }
}