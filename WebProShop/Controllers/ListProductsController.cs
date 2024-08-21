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
[Produces("application/json")]
public sealed class ListProductsController(IQueryable<Product> products) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ProductResult>>> Get([FromQuery]int pageSize = 20, [FromQuery]int page = 0)
    {
        var totalCount = await products.CountAsync();

        return new PagedResult<ProductResult>(products.Skip(page * pageSize).Take(pageSize).Select(Mapping.Product()).AsAsyncEnumerable(), totalCount);
    }


}
