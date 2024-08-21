using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using WebProShop.Data;
using WebProShop.Data.Models;
using WebProShop.Models;

namespace WebProShop.Controllers;

[ApiController]
[Route("products")]
[Tags("Product")]
[Produces(MediaTypeNames.Application.Json)]
public class DeleteProductController(IQueryable<Product> products, IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpDelete("{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResult>> Delete(Guid id)
    {
        var productToDelete = await products.SingleOrDefaultAsync(x => x.Id == id);

        if(productToDelete is null)
        {
            return NotFound();
        }

        unitOfWork.Delete(productToDelete);

        await unitOfWork.SaveChangesAsync();
        await unitOfWork.CommitAsync();

        return new ProductResult(productToDelete.Id, productToDelete.Name, productToDelete.Description, productToDelete.Price);
    }
}
