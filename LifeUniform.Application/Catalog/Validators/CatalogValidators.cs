using FluentValidation;
using LifeUniform.Application.Catalog.Commands;

namespace LifeUniform.Application.Catalog.Validators;

public class UpsertProductCommandValidator : AbstractValidator<UpsertProductCommand>
{
    public UpsertProductCommandValidator()
    {
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.DiscountPrice)
            .LessThan(x => x.Price)
            .When(x => x.DiscountPrice is not null);
    }
}

public class UpsertCategoryCommandValidator : AbstractValidator<UpsertCategoryCommand>
{
    public UpsertCategoryCommandValidator()
    {
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
