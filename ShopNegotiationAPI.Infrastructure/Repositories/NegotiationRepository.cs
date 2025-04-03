using Microsoft.EntityFrameworkCore;
using ShopNegotiationAPI.Application.Exceptions;
using ShopNegotiationAPI.Application.Interfaces.Repositories;
using ShopNegotiationAPI.Domain.Models;
using ShopNegotiationAPI.Infrastructure.Data;

namespace ShopNegotiationAPI.Infrastructure.Repositories;

public class NegotiationRepository : INegotiationRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<NegotiationRepository> _logger;
    public NegotiationRepository(AppDbContext context, ILogger<NegotiationRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<IEnumerable<Negotiation>> GetAllNegotiationsAsync()
    {
        try
        {
            return await _context.Negotiations.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving negotiations");
            throw;
        }
    }

    public async Task<Negotiation> GetNegotiationByIdAsync(int id)
    {
        try
        {
            var negotiation = await _context.Negotiations.FindAsync(id);
            if (negotiation == null)
            {
                _logger.LogWarning("Negotiation with id {Id} not found", id);
                throw new NegotiationNotFoundException(id);
            }
            return negotiation;
        } 
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving negotiation with id {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Negotiation>> GetNegotiationsByUserIdAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Negotiation>> GetNegotiationsByProductIdAsync(int productId)
    {
        try
        {
            var negotiations = await _context.Negotiations
                .Where(n => n.ProductId == productId)
                .ToListAsync();
            if (negotiations == null || !negotiations.Any())
            {
                _logger.LogWarning("No negotiations found for product with id {ProductId}", productId);
                throw new NegotiationNotFoundException(productId);
            }
            return negotiations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving negotiations for product with id {ProductId}", productId);
            throw;
        }
    }

    public async Task<Negotiation> AddNegotiationAsync(Negotiation negotiation)
    {
        try
        {
            if (negotiation == null)
            {
                _logger.LogWarning("Cannot add negotiation. Negotiation object is null");
                throw new ArgumentNullException(nameof(negotiation));
            }
            await _context.Negotiations.AddAsync(negotiation);
            await _context.SaveChangesAsync();
            return negotiation;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error adding negotiation");
            throw new Exception("An error occurred while adding the negotiation.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding negotiation");
            throw;
        }
    }

    public async Task<Negotiation> UpdateNegotiationAsync(Negotiation negotiation)
    {
        try
        {
            if (negotiation == null)
            {
                _logger.LogWarning("Cannot update negotiation. Negotiation object is null");
                throw new ArgumentNullException(nameof(negotiation));
            }
            _context.Negotiations.Update(negotiation);
            await _context.SaveChangesAsync();
            return negotiation;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Error updating negotiation");
            throw new Exception("An error occurred while updating the negotiation.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating negotiation");
            throw;
        }
    }

    public async Task<bool> DeleteNegotiationAsync(int id)
    {
        try
        {
            var negotiation = await _context.Negotiations.FindAsync(id);
            if (negotiation == null)
            {
                _logger.LogWarning("Negotiation with id {Id} not found", id);
                throw new NegotiationNotFoundException(id);
            }
            _context.Negotiations.Remove(negotiation);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error deleting negotiation");
            throw new Exception("An error occurred while deleting the negotiation.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting negotiation");
            throw;
        }
    }

    public async Task<bool> NegotiationExistsAsync(int id)
    {
        try
        {
            var exists = await _context.Negotiations.AnyAsync(n => n.Id == id);
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if negotiation exists with id {Id}", id);
            throw;
        }
    }
}