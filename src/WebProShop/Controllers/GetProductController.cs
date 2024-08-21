using Microsoft.AspNetCore.Http;
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
[Tags("Product")]
public sealed class GetProductController(IQueryable<Product> products) : ControllerBase
{
    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> Get([FromRoute]Guid id)
    {
        var item = await products.Where(x => x.Id == id).Select(Map).SingleOrDefaultAsync();

        return item switch
        {
            {} => Ok(item),
            _ => NotFound()
        };
    }

    private static readonly Expression<Func<Product, ProductResult>> Map = p => new ProductResult(p.Id, p.Name, p.Description, p.Price);
}
