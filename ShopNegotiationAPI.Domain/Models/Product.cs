namespace ShopNegotiationAPI.Domain.Models;

public class Product
{
    public int Id { get; set; }
    public string ProductName { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}