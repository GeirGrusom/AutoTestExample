using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MockQueryable.EntityFrameworkCore;
using NUnit.Framework;
using WebProShop.Controllers;
using WebProShop.Data.Models;
using WebProShop.Models;

namespace UnitTests;

public class GetProductControllerTests
{
    private class State
    {
        public Product TestProduct { get; } = new Product(Guid.NewGuid()) { Name = "Test", Description = "Test", Price = 10m };

        private IQueryable<Product> Products { get; }

        public State()
        {
            Products = new [] { TestProduct }.AsQueryable().BuildMock();
        }

        public GetProductController GetSubject() => new(Products);
    }

    private static (State state, GetProductController controller) Init()
    {
        var state = new State();
        return (state, state.GetSubject());
    }

    [Test]
    public async Task Get_ProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var (_, controller) = Init();

        // Act
        var result = await controller.Get(Guid.NewGuid());

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Get_ProductExists_ReturnsOk()
    {
        // Arrange
        var (state, controller) = Init();

        // Act
        var result = await controller.Get(state.TestProduct.Id);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Get_ProductExists_MapsToCorrectObject()
    {
        // Arrange
        var (state, controller) = Init();
        var expected = new ProductResult
        (
            state.TestProduct.Id,
            state.TestProduct.Name,
            state.TestProduct.Description, state.TestProduct.Price
        );

        // Act
        var result = await controller.Get(state.TestProduct.Id);

        // Assert
        Assert.That(result, Has.Property(nameof(OkObjectResult.Value)).EqualTo(expected));
    }
}
