using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebProShop.Data;
using WebProShop.Data.Models;
using WebProShop.Models;

namespace WebProShop.Controllers;

[ApiController]
[Route("shopping-carts")]
[Tags("Shopping cart")]
public sealed class CreateShoppingCartController(IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ShoppingCartResult>> CreateNew()
    {
        var shoppingCart = new ShoppingCart(Guid.NewGuid());
        unitOfWork.Add(shoppingCart);
        await unitOfWork.SaveChangesAsync();
        await unitOfWork.CommitAsync();

        return CreatedAtAction(nameof(GetShoppingCartController.GetShoppingCartById), new { id = shoppingCart.Id }, new ShoppingCartResult(shoppingCart.Id, []));
    }
}
