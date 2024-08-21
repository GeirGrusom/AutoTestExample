using System;
// ReSharper disable NotAccessedPositionalProperty.Global

namespace WebProShop.Models;

public record ProductResult(Guid Id, string Name, string Description, decimal Price);
