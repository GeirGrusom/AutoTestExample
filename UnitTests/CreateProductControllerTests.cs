using Microsoft.AspNetCore.Mvc;
using MockQueryable.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebProShop.Controllers;
using WebProShop.Data;
using WebProShop.Data.Models;

namespace UnitTests;

public class CreateProductControllerTests
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

        public CreateProductController GetSubject() => new(Products, UnitOfWork);
    }

    private static (State state, CreateProductController controller) Init()
    {
        var state = new State();
        return (state, state.GetSubject());
    }

    [Test]
    public async Task Create_ProductNameAlreadyExist_ReturnsConflict()
    {
        // Arrange
        var (_, controller) = Init();

        // Act
        var result = await controller.Create(new CreateProduct("Test", "Test", 100m));

        // Assert
        Assert.That(result, Is.InstanceOf<ConflictResult>());
    }

    [Test]
    public async Task Create_Ok_AddsObjectToUnitOfWork()
    {
        // Arrange
        var (state, controller) = Init();

        // Act
        _ = await controller.Create(new CreateProduct("Test New", "Test", 100m));

        // Assert
        state.UnitOfWork.Received().Add(Arg.Is<Product>(x => x.Name == "Test New"));
    }
}
