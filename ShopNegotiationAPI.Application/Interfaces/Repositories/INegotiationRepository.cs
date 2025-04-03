using ShopNegotiationAPI.Domain.Models;

namespace ShopNegotiationAPI.Application.Interfaces.Repositories;

public interface INegotiationRepository
{
    Task<IEnumerable<Negotiation>> GetAllNegotiationsAsync();
    Task<Negotiation> GetNegotiationByIdAsync(int id);
    Task<IEnumerable<Negotiation>> GetNegotiationsByUserIdAsync(int userId);
    Task<IEnumerable<Negotiation>> GetNegotiationsByProductIdAsync(int productId);
    Task<Negotiation> AddNegotiationAsync(Negotiation negotiation);
    Task<Negotiation> UpdateNegotiationAsync(Negotiation negotiation);
    Task<bool> DeleteNegotiationAsync(int id);
    Task<bool> NegotiationExistsAsync(int id);
}