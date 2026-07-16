using FluentValidation;
using Application.DTOs;

namespace Application.Validators
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("ProductName is required.")
                .MaximumLength(255);

            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithMessage("CreatedBy is required.")
                .MaximumLength(100);
        }
    }

    public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductDtoValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("ProductName is required.")
                .MaximumLength(255);

            RuleFor(x => x.ModifiedBy)
                .NotEmpty().WithMessage("ModifiedBy is required.")
                .MaximumLength(100);
        }
    }

    public class CreateItemDtoValidator : AbstractValidator<CreateItemDto>
    {
        public CreateItemDtoValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        }
    }
}