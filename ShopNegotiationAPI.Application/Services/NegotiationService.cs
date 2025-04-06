using ShopNegotiationAPI.Application.Interfaces.Repositories;
using ShopNegotiationAPI.Application.Interfaces.Services;
using ShopNegotiationAPI.Domain.Models;

namespace ShopNegotiationAPI.Application.Services;

public class NegotiationService : INegotiationService
{
    private readonly INegotiationRepository _negotiationRepository;

    public NegotiationService(INegotiationRepository negotiationRepository)
    {
        _negotiationRepository = negotiationRepository;
    }

    public async Task<IEnumerable<Negotiation>> GetAllNegotiationsAsync()
    {
        return await _negotiationRepository.GetAllNegotiationsAsync();
    }

    public async Task<Negotiation> GetNegotiationByIdAsync(int id)
    {
        return await _negotiationRepository.GetNegotiationByIdAsync(id);
    }

    public async Task<IEnumerable<Negotiation>> GetNegotiationsByProductIdAsync(int productId)
    {
        return await _negotiationRepository.GetNegotiationsByProductIdAsync(productId);
    }

    public async Task<Negotiation> AddNegotiationAsync(Negotiation negotiation)
    {
        return await _negotiationRepository.AddNegotiationAsync(negotiation);
    }

    public async Task<Negotiation> UpdateNegotiationAsync(Negotiation negotiation)
    {
        return await _negotiationRepository.UpdateNegotiationAsync(negotiation);
    }

    public async Task<bool> DeleteNegotiationAsync(int id)
    {
        return await _negotiationRepository.DeleteNegotiationAsync(id);
    }

    public async Task<bool> NegotiationExistsAsync(int id)
    {
        return await _negotiationRepository.NegotiationExistsAsync(id);
    }

    public async Task<Negotiation> ProcessNegotiationAsync(int negotiationId, decimal proposedPrice)
    {
        try
        {
            var negotiation = await _negotiationRepository.GetNegotiationByIdAsync(negotiationId);
            if (negotiation.AttemptsCount > 3)
            {
                negotiation.Status = NegotiationStatus.Closed;
                return negotiation;
            }

            negotiation.AttemptsCount++;
            negotiation.ProposedPrice = proposedPrice;
            return negotiation;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error processing negotiation with ID {negotiationId}: {ex.Message}", ex);
        }
    }

    public async Task<Negotiation> FinalizeNegotiationAsync(int negotiationId, bool isAccepted)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Negotiation>> GetPendingNegotiationsAsync()
    {
        try
        {
            var negotiations = await _negotiationRepository.GetAllNegotiationsAsync();
            var activeNegotiations = negotiations.Where(n => n.Status == NegotiationStatus.Pending).ToList();
            return activeNegotiations;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving pending negotiations: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<Negotiation>> GetExpiredNegotiationsAsync()
    {
        var negotiations = await _negotiationRepository.GetAllNegotiationsAsync();
        var expiredNegotiations = negotiations.Where(n => n.Status == NegotiationStatus.Expired).ToList();
        return expiredNegotiations;
    }
}