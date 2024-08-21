using System;
using System.Linq;
using System.Linq.Expressions;
using WebProShop.Controllers;
using WebProShop.Data.Models;

namespace WebProShop.Models;

public static class Mapping
{
    public static Expression<Func<Product, ProductResult>> Product() => p => new ProductResult(p.Id, p.Name, p.Description, p.Price);

    public static Expression<Func<ShoppingCart, ShoppingCartResult>> ShoppingCart() =>
        x => new ShoppingCartResult(x.Id, x.Lines.Select(l => new ShoppingCartItemResult(l.Id, l.Amount, l.Amount * l.Product.Price, l.Product.Name)).ToList());
}
