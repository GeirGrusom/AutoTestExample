using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebProShop.Data.Models;

public sealed class ShoppingCart(Guid id)
{
    public Guid Id { get; } = id;

    [MaxLength(100)]
    public List<ShoppingCartLine> Lines { get; } = [];
}
