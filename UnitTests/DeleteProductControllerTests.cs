using Microsoft.AspNetCore.Mvc;
using MockQueryable.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebProShop.Controllers;
using WebProShop.Data;
using WebProShop.Data.Models;

namespace UnitTests;

public class DeleteProductControllerTests
{
    private class State
    {
        public Product TestProduct { get; } = new Product(Guid.NewGuid()) { Name = "Test", Description = "Test", Price = 10m };

        private IQueryable<Product> Products { get; }

        public IUnitOfWork UnitOfWork { get; }

        public State()
        {
            Products = new [] { TestProduct }.AsQueryable().BuildMock();
            UnitOfWork = Substitute.For<IUnitOfWork>();
        }

        public DeleteProductController GetSubject() => new(Products, UnitOfWork);
    }

    private static (State state, DeleteProductController controller) Init()
    {
        var state = new State();
        return (state, state.GetSubject());
    }

    [Test]
    public async Task Delete_NoSuchThing_ReturnsNotFound()
    {
        // Arrange
        var (_, controller) = Init();

        // Act
        var result = await controller.Delete(Guid.Parse("ba42d2b7-4752-4f40-aa10-7eb881d24134"));

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Delete_Existing_ReturnsDeletedProduct()
    {
        // Arrange
        var (state, controller) = Init();

        // Act
        var result = await controller.Delete(state.TestProduct.Id);

        // Assert
        Assert.That(result.Value.Id, Is.EqualTo(state.TestProduct.Id));
    }

    [Test]
    public async Task Delete_Existing_CallsDeleteOnUnitOfWork()
    {
        // Arrange
        var (state, controller) = Init();

        // Act
        var result = await controller.Delete(state.TestProduct.Id);

        // Assert
        state.UnitOfWork.Received().Delete(state.TestProduct);
    }
}
