using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProShop.Data.Models
{
    public sealed record ShoppingCartLineRequest(Guid ProductId, int Amount);
    public sealed record UpdateShoppingCartRequest(Dictionary<Guid, int> Lines);
}
