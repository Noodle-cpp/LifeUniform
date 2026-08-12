using FluentAssertions;
using LifeUniform.Application.Orders.Commands;
using LifeUniform.Domain.Promotions;

namespace LifeUniform.Tests.Unit;

public class PromotionDiscountTests
{
    [Fact]
    public void CalculateDiscount_Percent_AppliesPercentage()
    {
        var promo = new PromotionCode
        {
            Type = PromotionDiscountType.Percent,
            Value = 15m,
            IsActive = true
        };

        CreateOrderHandler.CalculateDiscount(promo, 2000m).Should().Be(300m);
    }

    [Fact]
    public void CalculateDiscount_Fixed_ReturnsFixedAmount()
    {
        var promo = new PromotionCode
        {
            Type = PromotionDiscountType.Fixed,
            Value = 250m,
            IsActive = true
        };

        CreateOrderHandler.CalculateDiscount(promo, 2000m).Should().Be(250m);
    }

    [Fact]
    public void ValidatePromo_Expired_Throws()
    {
        var promo = new PromotionCode
        {
            Type = PromotionDiscountType.Percent,
            Value = 10m,
            IsActive = true,
            ValidTo = DateTimeOffset.UtcNow.AddDays(-1)
        };

        var act = () => CreateOrderHandler.ValidatePromo(promo, 1000m);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ValidatePromo_Inactive_Throws()
    {
        var promo = new PromotionCode
        {
            Type = PromotionDiscountType.Percent,
            Value = 10m,
            IsActive = false
        };

        var act = () => CreateOrderHandler.ValidatePromo(promo, 1000m);

        act.Should().Throw<InvalidOperationException>();
    }
}
