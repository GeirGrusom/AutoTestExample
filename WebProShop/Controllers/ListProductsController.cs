using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebProShop.Data.Models;
using WebProShop.Models;

namespace WebProShop.Controllers;

[ApiController]
[Route("products")]
public sealed class ListProductsController(IQueryable<Product> products) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery]int pageSize = 20, [FromQuery]int page = 0)
    {
        var totalCount = await products.CountAsync();

        return Ok(new PagedResult<ProductResult>( await products.Skip(page * pageSize).Take(pageSize).Select(Map).ToListAsync(), totalCount));
    }

    private static readonly Expression<Func<Product, ProductResult>> Map = p => new ProductResult(p.Id, p.Name, p.Description, p.Price);
}
