using MockQueryable.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebProShop.Controllers;
using WebProShop.Data;
using WebProShop.Data.Models;

namespace UnitTests;

public class UpdateShoppingCartControllerTests
{
    private class State
    {
        public Guid ShoppingCartId { get; } = Guid.NewGuid();
        public Product TestProduct { get; }  = new Product(Guid.NewGuid()) { Name = "Test", Description = "Test", Price = 10m };
        public IUnitOfWork UnitOfWork { get; } = Substitute.For<IUnitOfWork>();
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

        public UpdateShoppingCartController GetSubject()
        {
            return new(ShoppingCarts, UnitOfWork);
        }
    }

    private static (State state, UpdateShoppingCartController controller) Init()
    {
        var state = new State();
        return (state, state.GetSubject());
    }

    [Test]
    public async Task Put_Ok_ReturnsMappedItem()
    {
        // Arrange
        var (state, controller) = Init();

        // Act
        var result = await controller.Update(state.ShoppingCartId, new UpdateShoppingCartRequest(new() { [state.TestProduct.Id] = 5 }));

        // Assert
        var items = result.Value;
        Assert.That(items, Is.Not.Null);
        Assert.That(items.Lines, Has.One.Items);
    }
}
