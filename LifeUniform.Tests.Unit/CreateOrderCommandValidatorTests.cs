using FluentAssertions;
using FluentValidation.TestHelper;
using LifeUniform.Application.Orders.Commands;
using LifeUniform.Domain.Orders;

namespace LifeUniform.Tests.Unit;

public class CreateOrderCommandValidatorTests
{
    private readonly CreateOrderCommandValidator _validator = new();

    [Fact]
    public void Valid_Command_Passes()
    {
        var result = _validator.TestValidate(new CreateOrderCommand
        {
            CustomerName = "Иван",
            CustomerPhone = "+7999",
            CustomerEmail = "a@b.ru",
            DeliveryAddress = "Москва",
            DeliveryMethod = DeliveryMethod.Cdek
        });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_Email_Fails()
    {
        var result = _validator.TestValidate(new CreateOrderCommand
        {
            CustomerName = "Иван",
            CustomerPhone = "+7999",
            CustomerEmail = "",
            DeliveryAddress = "Москва",
            DeliveryMethod = DeliveryMethod.Cdek
        });

        result.ShouldHaveValidationErrorFor(x => x.CustomerEmail);
    }
}
