using System.Collections.Generic;

// ReSharper disable NotAccessedPositionalProperty.Global

namespace WebProShop.Models;

public sealed record PagedResult<T>(IAsyncEnumerable<T> Items, int TotalCount);
