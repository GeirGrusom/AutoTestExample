using System;
using System.Linq;
using WebProShop.Data.Models;
using WebProShop.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MockQueryable.EntityFrameworkCore;
using WebProShop.Models;

namespace UnitTests;

public class GetShoppingCartControllerTests
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

        public GetShoppingCartController GetSubject()
        {
            return new(ShoppingCarts);
        }
    }

    private static (State state, GetShoppingCartController controller) Init()
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
        Assert.That(result.Value, Is.Not.Null);
    }

    [Test]
    public async Task Get_EmptyId_ReturnsBadRequest()
    {
        // Arrange
        var (_, controller) = Init();

        // Act
        var result = await controller.GetShoppingCartById(default);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestResult>());
    }

    [Test]
    public async Task Get_ShoppingCartDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var (_, controller) = Init();

        // Act
        var result = await controller.GetShoppingCartById(Guid.NewGuid());

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Get_ShoppingCartExists_ReturnsShoppingCart()
    {
        // Arrange
        var (state, controller) = Init();

        // Act
        var result = await controller.GetShoppingCartById(state.ShoppingCartId);

        // Assert
        Assert.That(result.Value, Is.InstanceOf<ShoppingCartResult>());
    }

    [Test]
    public async Task Get_ShoppingCartExists_ReturnsShoppingCartWithLines()
    {
        // Arrange
        var (state, controller) = Init();

        // Act
        var result = await controller.GetShoppingCartById(state.ShoppingCartId) switch
        {
            { Value: {} shoppingCart } => shoppingCart,
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
            { Value: {} shoppingCart} => shoppingCart.Lines.Single(),
            _ => throw new AssertionException("Result did not return a shopping cart.")
        };

        // Assert
        Assert.That(result.TotalPrice, Is.EqualTo(20m)); // TotalPrice = 10 * 2
    }
}
