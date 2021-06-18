using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebProShop.Data.Models;
using WebProShop.Models;

namespace WebProShop.Controllers
{
    [ApiController]
    [Route("products")]
    public sealed class ProductsController : ControllerBase
    {
        private readonly IQueryable<Product> products;

        public ProductsController(IQueryable<Product> products)
        {
            this.products = products;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]int pageSize = 20, [FromQuery]int page = 0)
        {
            var totalCount = await products.CountAsync();

            return Ok(new PagedResult<ProductResult>( await products.Skip(page * pageSize).Take(pageSize).Select(Map).ToListAsync(), totalCount));
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> Get([FromRoute]Guid id)
        {
            var item = await products.Where(x => x.Id == id).Select(Map).SingleOrDefaultAsync();

            return item switch
            {
                ProductResult result => Ok(result),
                null => NotFound()
            };
        }

        private static readonly Expression<Func<Product, ProductResult>> Map = p => new ProductResult(p.Id, p.Name, p.Description, p.Price);
    }
}
