using FluentValidation;
using ShopNegotiationAPI.Domain.Models;

namespace ShopNegotiationAPI.Application.Validators;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(p => p.ProductName).NotEmpty().WithMessage("Product name is required");
        RuleFor(p => p.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
    }
}