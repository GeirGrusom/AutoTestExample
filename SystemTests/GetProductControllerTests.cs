using NUnit.Framework;
using System;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Serializers.Json;
using Testcontainers.PostgreSql;
using WebProShop;
using WebProShop.Models;

namespace SystemTests;

[NonParallelizable]
public class GetProductControllerTests
{
    private readonly System.Threading.CancellationTokenSource cancel = new ();
    private PostgreSqlContainer databaseContainer;
    private Task appTask;

    private static RestClient CreateClient() => new RestClient("http://localhost:8088/", configureSerialization: c => c.UseSystemTextJson());

    [OneTimeSetUp]
    public async Task Setup()
    {
        databaseContainer = new PostgreSqlBuilder()
            .WithImage("postgres:13.3-alpine")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithDatabase("postgres")
            .Build();

        await databaseContainer.StartAsync();

        appTask = Task.Run(() => Program.MainAsync(new[] { "--urls=http://*:8088/", "--environment=Development", "--ConnectionStrings:PostgreSql", databaseContainer.GetConnectionString() } , cancel.Token));

        await WaitForHealthCheck();

    }

    private async Task WaitForHealthCheck()
    {
        var client = CreateClient();
        var healthCheck = new RestRequest("/");
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
        await cancel.CancelAsync();
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

    [Test]
    public async Task Get_ReturnsPagedResult()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var result = await client.ExecuteGetAsync<PagedResult<ProductResult>>(new RestRequest("/products"));

        // Assert
        Assert.That(result.Data, Is.InstanceOf<PagedResult<ProductResult>>());
    }

    [Test]
    public async Task Get_ProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var client = CreateClient();
        var request = new RestRequest($"/products/{Guid.NewGuid():N}");

        // Act
        var result = await client.ExecuteAsync<PagedResult<ProductResult>>(request);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }
}
