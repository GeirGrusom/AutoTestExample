using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebProShop.Data;
using WebProShop.Data.Models;
using WebProShop.Models;

namespace WebProShop.Controllers;

[ApiController]
[Route("shopping-carts")]
[Produces("application/json")]
[Tags("Shopping cart")]
public sealed class UpdateShoppingCartController(IQueryable<ShoppingCart> shoppingCarts, IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpPut("{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ShoppingCartResult>> Update([FromRoute]Guid id, [FromBody] UpdateShoppingCartRequest request)
    {
        var shoppingCart = await shoppingCarts.SingleOrDefaultAsync(x => x.Id == id);

        if(shoppingCart is null)
        {
            return NotFound();
        }

        shoppingCart.Lines.Clear();
        unitOfWork.Update(shoppingCart);
        await unitOfWork.SaveChangesAsync();

        foreach (var (item, index) in request.Lines.Select((x,y) => (item: x, index:y)))
        {
            var newLine = new ShoppingCartLine(index) { Product = new Product(item.Key), Amount = item.Value };
            shoppingCart.Lines.Add(newLine);
        }

        unitOfWork.Update(shoppingCart);
        await unitOfWork.CommitAsync();

        return await shoppingCarts.Where(x => x.Id == id).Select(Mapping.ShoppingCart()).SingleAsync();
    }
}
