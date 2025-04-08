using FluentValidation;
using ShopNegotiationAPI.DTOs;

namespace ShopNegotiationAPI.Application.Validators
{
    public class ProductDTOValidator : AbstractValidator<ProductDTO>
    {
        public ProductDTOValidator()
        {
            RuleFor(dto => dto.ProductName)
                .NotEmpty()
                .WithMessage("Product name is required");
                
            RuleFor(dto => dto.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than 0");
                
            RuleFor(dto => dto.Quantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Quantity cannot be negative");
        }
    }
}