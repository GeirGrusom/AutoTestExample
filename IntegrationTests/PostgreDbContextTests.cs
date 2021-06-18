using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TestContainers.Container.Abstractions.Hosting;
using TestContainers.Container.Database.Hosting;
using TestContainers.Container.Database.PostgreSql;
using WebProShop.Data;
using WebProShop.Data.Migration;
using WebProShop.Data.Models;

namespace IntegrationTests
{
    public class PostgreDbContextTests : PostgreDbContextBase
    {
        

        [Test]
        public async Task Add_AddProduct()
        {
            // Arrange
            var id = Guid.NewGuid();
            using var ctx = GetContext();

            // Act
            ctx.Add(new Product(id) { Name = "Test", Description = "Bar", Price = 100m });
            await ctx.SaveChangesAsync();

            // Assert
            Assert.That(await ctx.Set<Product>().AnyAsync(x => x.Id == id), Is.True);
        }

        [Test]
        public async Task UnitOfWork_Commit_PersistsAdd()
        {
            // Arrange
            var id = Guid.NewGuid();
            using var ctx = GetContext();
            await using var unit = ctx.CreateUnitOfWork();

            // Act
            unit.Add(new ShoppingCart(id));
            await unit.SaveChangesAsync();
            await unit.CommitAsync();

            // Assert
            Assert.That(await ctx.Set<ShoppingCart>().AnyAsync(x => x.Id == id));
        }
    }
}
