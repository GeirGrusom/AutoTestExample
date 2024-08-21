using System;
using System.Collections.Generic;

namespace WebProShop.Models;

// ReSharper disable NotAccessedPositionalProperty.Global
public sealed record ShoppingCartResult(Guid Id, List<ShoppingCartItemResult> Lines);
// ReSharper restore NotAccessedPositionalProperty.Global
