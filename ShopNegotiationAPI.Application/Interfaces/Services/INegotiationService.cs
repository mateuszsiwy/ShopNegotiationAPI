using ShopNegotiationAPI.Domain.Models;

namespace ShopNegotiationAPI.Application.Interfaces.Services;

public interface INegotiationService
{
    Task<IEnumerable<Negotiation>> GetAllNegotiationsAsync();
    Task<Negotiation> GetNegotiationByIdAsync(int id);
    Task<IEnumerable<Negotiation>> GetNegotiationsByProductIdAsync(int productId);
    Task<Negotiation> AddNegotiationAsync(Negotiation negotiation);
    Task<Negotiation> UpdateNegotiationAsync(Negotiation negotiation);
    Task<bool> DeleteNegotiationAsync(int id);
    Task<bool> NegotiationExistsAsync(int id);
    
    Task<Negotiation> ProcessNegotiationAsync(int negotiationId, decimal proposedPrice);
    Task<Negotiation> FinalizeNegotiationAsync(int negotiationId, bool isAccepted);
    Task<IEnumerable<Negotiation>> GetActiveNegotiationsAsync();
    Task<IEnumerable<Negotiation>> GetExpiredNegotiationsAsync();
}