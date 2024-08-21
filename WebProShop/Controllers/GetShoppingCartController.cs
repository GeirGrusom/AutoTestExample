using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
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
            return NotFound();
        }

        return result;
    }
}
