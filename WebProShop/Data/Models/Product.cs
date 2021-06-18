using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProShop.Data.Models
{
    public sealed class Product
    {
        public Product(Guid id)
        {
            Id = id;
            Name = "";
            Description = "";
        }

        public Guid Id { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
