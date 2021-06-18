using System;

namespace WebProShop.Controllers
{
    public record ProductResult(Guid id, string Name, string Description, decimal Price);
}
