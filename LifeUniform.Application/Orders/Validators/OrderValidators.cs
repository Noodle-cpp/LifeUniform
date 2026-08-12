using FluentValidation;

namespace LifeUniform.Application.Orders.Commands;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CustomerPhone)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^\+7 \(\d{3}\) \d{3}-\d{2}-\d{2}$")
            .WithMessage("Введите телефон в формате +7 (900) 000-00-00");
        RuleFor(x => x.CustomerEmail).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.DeliveryAddress).NotEmpty().MaximumLength(500);
        RuleFor(x => x.DeliveryMethod).IsInEnum();
        RuleFor(x => x.PromoCode).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.PromoCode));
    }
}

public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
{
    public AddToCartCommandValidator()
    {
        RuleFor(x => x.ProductSlug).NotEmpty();
        RuleFor(x => x.SizeId)
            .NotEmpty()
            .WithMessage("Выберите размер перед добавлением в корзину.");
        RuleFor(x => x.Quantity).InclusiveBetween(1, 99);
    }
}
