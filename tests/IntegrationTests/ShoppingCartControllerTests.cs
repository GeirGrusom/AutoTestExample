﻿using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebProShop.Controllers;
using WebProShop.Data;
using WebProShop.Data.Models;

namespace IntegrationTests;

public class ShoppingCartControllerTests : PostgreDbContextBase
{
    private class State
    {
        public PgDataContext Context { get; }

        public Product ExampleProduct { get; }

        public State(PgDataContext context)
        {
            Context = context;
            ExampleProduct = new Product(Guid.NewGuid()) { Name = "Foo Soap", Description = "Soap with smell of foo", Price = 10m };
        }

        public UpdateShoppingCartController GetSubject()
        {
            Context.Add(ExampleProduct);
            Context.SaveChanges();
            return new UpdateShoppingCartController(Context.Set<ShoppingCart>(), Context.CreateUnitOfWork());
        }
    }

    (UpdateShoppingCartController controller, State state) Init()
    {
        var ctx = GetContext();
        var state = new State(ctx);
        return (state.GetSubject(), state);
    }

    [Test]
    public async Task Put_UpdatesShoppingCart()
    {
            // Arrange
            var (controller, state) = Init();
            var shoppingCart = new ShoppingCart(Guid.NewGuid()) { Lines = { new(0) { Amount = 10, Product = state.Context.Set<Product>().Single(x => x.Id == state.ExampleProduct.Id) } } };
            state.Context.Add(shoppingCart);
            await state.Context.SaveChangesAsync();

            // Act
            await controller.Update(shoppingCart.Id, new UpdateShoppingCartRequest(new() { [state.ExampleProduct.Id] = 5 }));

            // Assert
            var result = state.Context.Set<ShoppingCart>().Single(s => s.Id == shoppingCart.Id);
            Assert.That(result.Lines[0].Amount, Is.EqualTo(5));
        }


}
