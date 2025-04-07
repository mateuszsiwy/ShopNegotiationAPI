namespace ShopNegotiationAPI.DTOs;

public class ProductDTO
{
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}