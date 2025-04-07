using FluentValidation;
using ShopNegotiationAPI.Domain.Models;

namespace ShopNegotiationAPI.Application.Validators;

public class NegotiationValidator : AbstractValidator<Negotiation>
{
    public NegotiationValidator()
    {
        RuleFor(n => n.ProposedPrice).GreaterThan(0).WithMessage("Proposed price must be greater than 0");
        RuleFor(n => n.ProductId).GreaterThan(0).WithMessage("Valid product ID is required");
    }
}