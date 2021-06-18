using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProShop.Data.Models
{
    public sealed class ShoppingCart
    {
        public ShoppingCart(Guid id)
        {
            Id = id;
            Lines = new List<ShoppingCartLine>();
        }

        public Guid Id { get; }

        public List<ShoppingCartLine> Lines { get; }
    }
}
