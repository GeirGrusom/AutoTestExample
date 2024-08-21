using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebProShop.Data;
using WebProShop.Data.Models;

namespace WebProShop.Controllers;

[ApiController]
[Route("shopping-carts")]
public sealed class ShoppingCartController(IQueryable<ShoppingCart> shoppingCarts) : ControllerBase
{
    [HttpGet("{id:Guid}", Name = "GetShoppingCartById")]
    public async Task<IActionResult> GetShoppingCartById(Guid id)
    {
        if(id == default)
        {
            return BadRequest();
        }

        var result = await shoppingCarts
            .Where(x => x.Id == id)
            .Select(Map)
            .SingleOrDefaultAsync();

        if(result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromServices]IUnitOfWork unitOfWork)
    {
        var shoppingCart = new ShoppingCart(Guid.NewGuid());
        unitOfWork.Add(shoppingCart);
        await unitOfWork.SaveChangesAsync();
        await unitOfWork.CommitAsync();

        return CreatedAtAction(nameof(GetShoppingCartById), new { id = shoppingCart.Id }, new ShoppingCartResult(shoppingCart.Id, new List<ShoppingCartItemResult>()));
    }

    [HttpPut("{id:Guid}")]
    public async Task<IActionResult> Put([FromRoute]Guid id, [FromBody] UpdateShoppingCartRequest request, [FromServices]IUnitOfWork unitOfWork)
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

        return Ok(await shoppingCarts.Where(x => x.Id == id).Select(Map).SingleAsync());
    }

    private static readonly Expression<Func<ShoppingCart, ShoppingCartResult>> Map = x => new ShoppingCartResult(x.Id, x.Lines.Select(l => new ShoppingCartItemResult(l.Id, l.Amount, l.Amount * l.Product.Price, l.Product.Name)).ToList());
}

// ReSharper disable NotAccessedPositionalProperty.Global
public sealed record ShoppingCartItemResult(int Id, int Amount, decimal TotalPrice, string Name);

public sealed record ShoppingCartResult(Guid Id, List<ShoppingCartItemResult> Lines);

// ReSharper restore NotAccessedPositionalProperty.Global
