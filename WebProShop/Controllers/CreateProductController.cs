using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebProShop.Data;
using WebProShop.Data.Models;
using WebProShop.Models;

namespace WebProShop.Controllers;

[ApiController]
[Route("products")]
public sealed class CreateProductController(IQueryable<Product> products, IUnitOfWork unitOfWork) : ControllerBase
{
    [ProducesResponseType<ProductResult>(StatusCodes.Status201Created)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateProduct product)
    {
        if(await products.AnyAsync(x => x.Name.ToLower() == product.Name.ToLower()))
        {
            return Conflict();
        }

        var result = new Product(Guid.NewGuid()) { Name = product.Name, Description = product.Description, Price = product.Price };
        unitOfWork.Add(result);

        await unitOfWork.SaveChangesAsync();

        return Created($"products/{result.Id:N}", new ProductResult(result.Id, result.Name, result.Description, result.Price));
    }
}

public sealed record CreateProduct([MaxLength(100), MinLength(2)]string Name, [MaxLength(250)] string Description, [Range(typeof(decimal), "0", "10000")] decimal Price);
