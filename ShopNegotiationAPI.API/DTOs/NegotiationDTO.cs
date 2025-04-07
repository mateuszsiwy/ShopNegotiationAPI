namespace ShopNegotiationAPI.DTOs;

public class NegotiationDTO
{
    public int ProductId { get; set; }
    public string NegotiatorName { get; set; } = string.Empty;
    public decimal ProposedPrice { get; set; }
}