using NUnit.Framework;
using System;
using System.Linq;
using WebProShop.Data.Models;
using WebProShop.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MockQueryable.EntityFrameworkCore;
using NSubstitute;
using WebProShop.Data;

namespace UnitTests;

public class ShoppingCartControllerTests
{
    private class State
    {
        public Guid ShoppingCartId { get; } = Guid.NewGuid();
        public Product TestProduct { get; }  = new Product(Guid.NewGuid()) { Name = "Test", Description = "Test", Price = 10m };

        private IQueryable<ShoppingCart> ShoppingCarts { get; }

        public State()
        {
            ShoppingCarts = new ShoppingCart[]
            {
                new (ShoppingCartId)
                {
                    Lines =
                    {
                        new(0)
                        {
                            Product = TestProduct,
                            Amount = 2
                        }
                    }
                }
            }.AsQueryable().BuildMock();
        }

        public ShoppingCartController GetSubject()
        {
            return new(ShoppingCarts);
        }
    }

    static (State state, ShoppingCartController controller) Init()
    {
        var state = new State();
        return (state, state.GetSubject());
    }

    [Test]
    public async Task Get_ShoppingCartExists_ReturnsOk()
    {
        // Arrange
        var (state, controller) = Init();

        // Act
        var result = await controller.GetShoppingCartById(state.ShoppingCartId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Get_EmptyId_ReturnsBadRequest()
    {
        // Arrange
        var (_, controller) = Init();

        // Act
        var result = await controller.GetShoppingCartById(default);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestResult>());
    }

    [Test]
    public async Task Get_ShoppingCartDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var (_, controller) = Init();

        // Act
        var result = await controller.GetShoppingCartById(Guid.NewGuid());

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Get_ShoppingCartExists_ReturnsShoppingCart()
    {
        // Arrange
        var (state, controller) = Init();

        // Act
        var result = await controller.GetShoppingCartById(state.ShoppingCartId);

        // Assert
        Assert.That(result, Has.Property(nameof(OkObjectResult.Value)).InstanceOf<ShoppingCartResult>());
    }

    [Test]
    public async Task Get_ShoppingCartExists_ReturnsShoppingCartWithLines()
    {
        // Arrange
        var (state, controller) = Init();

        // Act
        var result = await controller.GetShoppingCartById(state.ShoppingCartId) switch
        {
            OkObjectResult { Value: ShoppingCartResult shoppingCart } => shoppingCart,
            _ => throw new AssertionException("Result did not return a shopping cart.")
        };

        // Assert
        Assert.That(result.Lines, Has.One.Items);
    }

    [Test]
    public async Task Get_ShoppingCartLineExists_ReturnsTotalPriceAsProductOfAmountAndPrice()
    {
        // Arrange
        var (state, controller) = Init();

        // Act
        var result = await controller.GetShoppingCartById(state.ShoppingCartId) switch
        {
            OkObjectResult { Value: ShoppingCartResult shoppingCart} => shoppingCart.Lines.Single(),
            _ => throw new AssertionException("Result did not return a shopping cart.")
        };

        // Assert
        Assert.That(result.TotalPrice, Is.EqualTo(20m)); // TotalPrice = 10 * 2
    }

    [Test]
    public async Task Post_Ok_CreatesNewShoppingCartWithValidId()
    {
        // Arrange
        var (_, controller) = Init();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        // Act
        await controller.Post(unitOfWork);

        // Assert
        unitOfWork.Received().Add(Arg.Is<ShoppingCart>(s => s.Id != default));
    }

    [Test]
    public async Task Post_Ok_ReturnsCreatedResponse()
    {
        // Arrange
        var (_, controller) = Init();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        // Act
        var result = await controller.Post(unitOfWork);

        // Assert
        Assert.That(result, Is.InstanceOf<CreatedAtActionResult>().With.Property(nameof(CreatedAtActionResult.ActionName)).EqualTo("GetShoppingCartById"));
    }

    [Test]
    public async Task Post_Ok_ReturnsNewId()
    {
        // Arrange
        var (_, controller) = Init();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        // Act
        var result = await controller.Post(unitOfWork);

        // Assert
        Assert.That(result, Has.Property(nameof(CreatedAtActionResult.RouteValues)).With.ContainKey("id"));
    }

    [Test]
    public async Task Put_Ok_ReturnsMappedItem()
    {
        // Arrange
        var (state, controller) = Init();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        // Act
        var result = await controller.Put(state.ShoppingCartId, new UpdateShoppingCartRequest(new() { [state.TestProduct.Id] = 5 }), unitOfWork);

        // Assert
        var items = (ShoppingCartResult)((OkObjectResult)result).Value;
        Assert.That(items, Is.Not.Null);
        Assert.That(items.Lines, Has.One.Items);
    }
}
