using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebProShop.Controllers;
using WebProShop.Data;
using WebProShop.Data.Models;

namespace UnitTests;


public class CreateShoppingCartControllerTests
{
    private class State
    {
        public IUnitOfWork UnitOfWork { get; } = Substitute.For<IUnitOfWork>();

        public CreateShoppingCartController GetSubject() => new(UnitOfWork);
    }

    private static (State state, CreateShoppingCartController controller) Init()
    {
        var state = new State();
        return (state, state.GetSubject());
    }

    [Test]
    public async Task Post_Ok_CreatesNewShoppingCartWithValidId()
    {
        // Arrange
        var (state, controller) = Init();

        // Act
        await controller.CreateNew();

        // Assert
        state.UnitOfWork.Received().Add(Arg.Is<ShoppingCart>(s => s.Id != default));
    }

    [Test]
    public async Task Post_Ok_ReturnsCreatedResponse()
    {
        // Arrange
        var (_, controller) = Init();

        // Act
        var result = await controller.CreateNew();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>().With.Property(nameof(CreatedAtActionResult.ActionName)).EqualTo("GetShoppingCartById"));
    }

    [Test]
    public async Task Post_Ok_ReturnsNewId()
    {
        // Arrange
        var (_, controller) = Init();

        // Act
        var result = await controller.CreateNew();

        // Assert
        Assert.That(result.Result, Has.Property(nameof(CreatedAtActionResult.RouteValues)).With.ContainKey("id"));
    }
}
