namespace WebProShop.Data.Models;

public sealed class ShoppingCartLine(int id)
{

    public int Id { get; } = id;
    public required Product Product { get; init; }
    public int Amount { get; set; }
}
