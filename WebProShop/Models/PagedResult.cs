using System.Collections.Generic;

namespace WebProShop.Models
{
    public sealed record PagedResult<T>(List<T> Items, int TotalCount);
}
