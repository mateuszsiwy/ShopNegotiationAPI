namespace ShopNegotiationAPI.Domain.Models;

public class Negotiation
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string NegotiatorName { get; set; } = String.Empty;
    public decimal InitialPrice { get; set; }
    public decimal ProposedPrice { get; set; }
    public decimal FinalPrice { get; set; }
    public DateTime NegotiationDate { get; set; } = DateTime.UtcNow;
    public NegotiationStatus Status { get; set; } = NegotiationStatus.Pending;
    public int AttemptsCount { get; set; } = 0;
    public DateTime? LastResponseDate { get; set; } 
    public DateTime? ExpirationDate { get; set; } 
}