using System;
using System.ComponentModel.DataAnnotations;

namespace WebProShop.Data.Models;

public sealed class Product(Guid id)
{
    public Guid Id { get; } = id;

    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(250)]
    public string Description { get; set; }

    [Range(typeof(decimal), minimum: "0", maximum: "1000000")]
    public decimal Price { get; set; }
}
