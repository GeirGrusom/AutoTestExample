using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebProShop.Data.Models;
using WebProShop.Models;

namespace WebProShop.Controllers;

[ApiController]
[Route("shopping-carts")]
[Tags("Shopping cart")]
[Produces("application/json")]
public sealed class GetShoppingCartController(IQueryable<ShoppingCart> shoppingCarts) : ControllerBase
{
    [HttpGet("{id:Guid}", Name = nameof(GetShoppingCartById))]
    [ProducesErrorResponseType(typeof(void))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingCartResult>> GetShoppingCartById(Guid id)
    {
        if(id == default)
        {
            return BadRequest();
        }

        var result = await shoppingCarts
            .Where(x => x.Id == id)
            .Select(Mapping.ShoppingCart())
            .SingleOrDefaultAsync();

        if(result is null)
        {
            return NotFound(new ProblemDetails { Title = "Shopping cart not found", Status = 404, Detail = $"Shopping cart with id {id:N} could not be found."});
        }

        return result;
    }
}
