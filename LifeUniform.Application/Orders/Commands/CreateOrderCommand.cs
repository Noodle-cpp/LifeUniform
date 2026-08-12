using LifeUniform.Application.Abstractions.Delivery;
using LifeUniform.Application.Orders.Dto;
using LifeUniform.Domain.Cart;
using LifeUniform.Domain.Orders;
using LifeUniform.Domain.Promotions;
using MediatR;

namespace LifeUniform.Application.Orders.Commands;

public class CreateOrderCommand : IRequest<OrderDto>
{
    public string? UserId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerPhone { get; init; } = string.Empty;
    public string CustomerEmail { get; init; } = string.Empty;
    public string DeliveryAddress { get; init; } = string.Empty;
    public DeliveryMethod DeliveryMethod { get; init; }
    public string? PromoCode { get; init; }
}

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly ICartService _cart;
    private readonly IOrderRepository _orders;
    private readonly IPromotionRepository _promotions;
    private readonly IDeliveryCalculator _delivery;

    public CreateOrderHandler(
        ICartService cart,
        IOrderRepository orders,
        IPromotionRepository promotions,
        IDeliveryCalculator delivery)
    {
        _cart = cart;
        _orders = orders;
        _promotions = promotions;
        _delivery = delivery;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var cartItems = _cart.GetItems();
        if (cartItems.Count == 0)
            throw new InvalidOperationException("Корзина пуста.");

        var itemsTotal = cartItems.Sum(x => x.UnitPrice * x.Quantity);
        var deliveryFee = _delivery.CalculateFee(request.DeliveryMethod, itemsTotal);

        decimal discount = 0m;
        string? appliedPromo = null;
        PromotionCode? promoEntity = null;

        if (!string.IsNullOrWhiteSpace(request.PromoCode))
        {
            promoEntity = await _promotions.GetByCodeAsync(request.PromoCode, cancellationToken)
                ?? throw new InvalidOperationException("Промокод не найден.");

            ValidatePromo(promoEntity, itemsTotal);
            discount = CalculateDiscount(promoEntity, itemsTotal);
            appliedPromo = promoEntity.Code;
        }

        if (discount > itemsTotal)
            discount = itemsTotal;

        var order = new Order
        {
            Id = Guid.NewGuid(),
            Number = $"LU-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}",
            UserId = request.UserId,
            CustomerName = request.CustomerName.Trim(),
            CustomerPhone = request.CustomerPhone.Trim(),
            CustomerEmail = request.CustomerEmail.Trim(),
            DeliveryAddress = request.DeliveryAddress.Trim(),
            DeliveryMethod = request.DeliveryMethod,
            Status = OrderStatus.PendingPayment,
            ItemsTotal = itemsTotal,
            DiscountAmount = discount,
            PromoCode = appliedPromo,
            DeliveryFee = deliveryFee,
            GrandTotal = itemsTotal - discount + deliveryFee,
            PaymentToken = Guid.NewGuid().ToString("N"),
            CreatedAt = DateTimeOffset.UtcNow,
            Items = cartItems.Select(x => new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                ProductSlug = x.ProductSlug,
                SizeId = x.SizeId,
                SizeLabel = x.SizeLabel,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice
            }).ToList()
        };

        foreach (var item in order.Items)
            item.OrderId = order.Id;

        await _orders.CreateAsync(order, cancellationToken);

        if (promoEntity is not null)
            await _promotions.IncrementRedemptionAsync(promoEntity.Id, cancellationToken);

        _cart.Clear();
        return Map(order);
    }

    public static void ValidatePromo(PromotionCode promo, decimal itemsTotal)
    {
        if (!promo.IsActive)
            throw new InvalidOperationException("Промокод неактивен.");

        var now = DateTimeOffset.UtcNow;
        if (promo.ValidFrom is not null && now < promo.ValidFrom)
            throw new InvalidOperationException("Промокод ещё не действует.");
        if (promo.ValidTo is not null && now > promo.ValidTo)
            throw new InvalidOperationException("Срок действия промокода истёк.");
        if (promo.MinOrderAmount is not null && itemsTotal < promo.MinOrderAmount)
            throw new InvalidOperationException($"Минимальная сумма заказа для промокода: {promo.MinOrderAmount:0.##} руб.");
        if (promo.MaxRedemptions is not null && promo.RedemptionCount >= promo.MaxRedemptions)
            throw new InvalidOperationException("Лимит использований промокода исчерпан.");
    }

    public static decimal CalculateDiscount(PromotionCode promo, decimal itemsTotal) =>
        promo.Type switch
        {
            PromotionDiscountType.Percent => Math.Round(itemsTotal * (promo.Value / 100m), 2),
            PromotionDiscountType.Fixed => promo.Value,
            _ => 0m
        };

    public static OrderDto Map(Order order) => new()
    {
        Id = order.Id,
        Number = order.Number,
        CustomerName = order.CustomerName,
        CustomerPhone = order.CustomerPhone,
        CustomerEmail = order.CustomerEmail,
        DeliveryAddress = order.DeliveryAddress,
        DeliveryMethod = order.DeliveryMethod,
        Status = order.Status,
        ItemsTotal = order.ItemsTotal,
        DiscountAmount = order.DiscountAmount,
        PromoCode = order.PromoCode,
        DeliveryFee = order.DeliveryFee,
        GrandTotal = order.GrandTotal,
        PaymentToken = order.PaymentToken,
        CreatedAt = order.CreatedAt,
        Items = order.Items.Select(i => new OrderItemDto
        {
            ProductName = i.ProductName,
            ProductSlug = i.ProductSlug,
            SizeLabel = i.SizeLabel,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice
        }).ToList()
    };
}
