using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MockQueryable.NSubstitute;
using NUnit.Framework;
using WebProShop.Controllers;
using WebProShop.Data.Models;

namespace UnitTests
{
    public class ProductsControllerTests
    {
        class State
        {
            public Product TestProduct { get; } = new Product(Guid.NewGuid()) { Name = "Test", Description = "Test", Price = 10m };


            public IQueryable<Product> Products { get; }

            public State()
            {
                Products = new [] { TestProduct }.AsQueryable().BuildMock();
            }

            public ProductsController GetSubject()
            {
                return new(Products);
            }
        }

        static (State state, ProductsController controller) Init()
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
}
