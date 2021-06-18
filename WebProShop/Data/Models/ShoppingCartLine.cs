using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProShop.Data.Models
{
    public sealed class ShoppingCartLine
    {
        public ShoppingCartLine(int id)
        {
            Id = id;
        }

        public int Id { get; }
        public Product Product { get; init; }
        public int Amount { get; set; }
    }
}
