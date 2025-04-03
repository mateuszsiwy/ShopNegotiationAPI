using ShopNegotiationAPI.Domain.Models;

namespace ShopNegotiationAPI.Application.Exceptions;

public class NegotiationNotFoundException : Exception
{
    public NegotiationNotFoundException(int id)
        : base($"Negotiation with ID {id} not found.")
    {
    }
}