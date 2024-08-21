using System;
using System.Collections.Generic;

namespace WebProShop.Data.Models;

public sealed record ShoppingCartLineRequest(Guid ProductId, int Amount);
public sealed record UpdateShoppingCartRequest(Dictionary<Guid, int> Lines);