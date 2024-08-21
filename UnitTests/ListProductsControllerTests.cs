using Microsoft.AspNetCore.Mvc;
using MockQueryable.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebProShop.Controllers;
using WebProShop.Data.Models;
using WebProShop.Models;

namespace UnitTests;

public class ListProductsControllerTests
{
    private class State
    {
        public Product TestProduct1 { get; } = new Product(Guid.NewGuid()) { Name = "Test1", Description = "Test1", Price = 10m };
        public Product TestProduct2 { get; } = new Product(Guid.NewGuid()) { Name = "Test2", Description = "Test2", Price = 20m };
        public Product TestProduct3 { get; } = new Product(Guid.NewGuid()) { Name = "Test3", Description = "Test3", Price = 30m };

        private IQueryable<Product> Products { get; }

        public State()
        {
            Products = new [] {TestProduct1, TestProduct2, TestProduct3}.AsQueryable().BuildMock();
        }

        public ListProductsController GetSubject() => new(Products);
    }

    private static (State state, ListProductsController controller) Init()
    {
        var state = new State();
        return (state, state.GetSubject());
    }

    [Test]
    public async Task Get_TakeOne_ReturnsOkObjectResult()
    {
        // Arrange
        var (_, controller) = Init();

        // Act
        var result = await controller.Get(1, 0);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Get_TakeOne_ReturnsPagedResult()
    {
        // Arrange
        var (_, controller) = Init();

        // Act
        var result = await controller.Get(1, 0);

        // Assert
        var item = ((OkObjectResult)result).Value;
        Assert.That(item, Is.InstanceOf<PagedResult<ProductResult>>());
    }

    [Test]
    public async Task Get_TakeOne_ReturnsFirstProductResult()
    {
        // Arrange
        var (state, controller) = Init();
        var expected = new ProductResult(state.TestProduct1.Id, state.TestProduct1.Name, state.TestProduct1.Description, state.TestProduct1.Price);

        // Act
        var result = await controller.Get(1, 0);

        // Assert
        var item = (PagedResult<ProductResult>)((OkObjectResult)result).Value;
        Assert.That(item!.Items, Is.EquivalentTo(new [] { expected }));
    }
}
