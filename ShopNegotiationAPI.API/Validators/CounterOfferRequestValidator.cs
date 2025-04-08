using FluentValidation;
using ShopNegotiationAPI.DTOs;

namespace ShopNegotiationAPI.Application.Validators
{
    public class CounterOfferRequestValidator : AbstractValidator<CounterOfferRequest>
    {
        public CounterOfferRequestValidator()
        {
            RuleFor(dto => dto.ProposedPrice)
                .GreaterThan(0)
                .WithMessage("Counter offer price must be greater than 0");
        }
    }
}