using FluentValidation;
using ShopNegotiationAPI.DTOs;

namespace ShopNegotiationAPI.Application.Validators;

public class NegotiationDTOValidator : AbstractValidator<NegotiationDTO>
{
    public NegotiationDTOValidator()
    {
        RuleFor(dto => dto.ProposedPrice)
            .GreaterThan(0)
            .WithMessage("Proposed price must be greater than 0");

        RuleFor(dto => dto.ProductId)
            .GreaterThan(0)
            .WithMessage("Valid product ID is required");

        RuleFor(dto => dto.NegotiatorName)
            .NotEmpty()
            .WithMessage("Negotiator name is required");
    }
}