using FluentAssertions;
using LifeUniform.Application.Abstractions.Delivery;
using LifeUniform.Application.Orders.Commands;
using LifeUniform.Domain.Cart;
using LifeUniform.Domain.Orders;
using LifeUniform.Domain.Promotions;
using Moq;

namespace LifeUniform.Tests.Unit;

public class CreateOrderHandlerTests
{
    private readonly Mock<ICartService> _cart = new();
    private readonly Mock<IOrderRepository> _orders = new();
    private readonly Mock<IPromotionRepository> _promotions = new();
    private readonly Mock<IDeliveryCalculator> _delivery = new();

    private CreateOrderHandler CreateSut() =>
        new(_cart.Object, _orders.Object, _promotions.Object, _delivery.Object);

    private static CreateOrderCommand ValidCommand(string? promo = null) => new()
    {
        CustomerName = "Test User",
        CustomerPhone = "+79001234567",
        CustomerEmail = "test@example.com",
        DeliveryAddress = "Moscow",
        DeliveryMethod = DeliveryMethod.Cdek,
        PromoCode = promo
    };

    [Fact]
    public async Task Handle_EmptyCart_Throws()
    {
        _cart.Setup(c => c.GetItems()).Returns(Array.Empty<CartLine>());

        var act = () => CreateSut().Handle(ValidCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        _orders.Verify(o => o.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CreatesOrder_WithDeliveryFeeFromCalculator()
    {
        var productId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        _cart.Setup(c => c.GetItems()).Returns(new[]
        {
            new CartLine
            {
                ProductId = productId,
                ProductName = "Scrubs",
                ProductSlug = "scrubs",
                SizeId = sizeId,
                SizeLabel = "M",
                UnitPrice = 1000m,
                Quantity = 2
            }
        });
        _delivery.Setup(d => d.CalculateFee(DeliveryMethod.Cdek, 2000m)).Returns(350m);
        Order? saved = null;
        _orders.Setup(o => o.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Callback<Order, CancellationToken>((o, _) => saved = o)
            .ReturnsAsync((Order o, CancellationToken _) => o);

        var result = await CreateSut().Handle(ValidCommand(), CancellationToken.None);

        result.ItemsTotal.Should().Be(2000m);
        result.DeliveryFee.Should().Be(350m);
        result.DiscountAmount.Should().Be(0m);
        result.GrandTotal.Should().Be(2350m);
        saved.Should().NotBeNull();
        saved!.DeliveryFee.Should().Be(350m);
        _cart.Verify(c => c.Clear(), Times.Once);
        _delivery.Verify(d => d.CalculateFee(DeliveryMethod.Cdek, 2000m), Times.Once);
    }

    [Fact]
    public async Task Handle_AppliesPercentPromo_Welcome10()
    {
        _cart.Setup(c => c.GetItems()).Returns(new[]
        {
            new CartLine
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Scrubs",
                ProductSlug = "scrubs",
                SizeId = Guid.NewGuid(),
                SizeLabel = "L",
                UnitPrice = 1000m,
                Quantity = 1
            }
        });
        _delivery.Setup(d => d.CalculateFee(It.IsAny<DeliveryMethod>(), It.IsAny<decimal>())).Returns(100m);

        var promo = new PromotionCode
        {
            Id = Guid.NewGuid(),
            Code = "WELCOME10",
            Type = PromotionDiscountType.Percent,
            Value = 10m,
            IsActive = true
        };
        _promotions.Setup(p => p.GetByCodeAsync("WELCOME10", It.IsAny<CancellationToken>()))
            .ReturnsAsync(promo);
        _orders.Setup(o => o.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order o, CancellationToken _) => o);

        var result = await CreateSut().Handle(ValidCommand("WELCOME10"), CancellationToken.None);

        result.DiscountAmount.Should().Be(100m);
        result.PromoCode.Should().Be("WELCOME10");
        result.GrandTotal.Should().Be(1000m - 100m + 100m);
        _promotions.Verify(p => p.IncrementRedemptionAsync(promo.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
