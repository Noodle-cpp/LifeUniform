using LifeUniform.Domain.Orders;

namespace LifeUniform.Application.Orders.Dto;

public class OrderItemDto
{
    public string ProductName { get; set; } = string.Empty;
    public string ProductSlug { get; set; } = string.Empty;
    public string SizeLabel { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
}

public class OrderDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public DeliveryMethod DeliveryMethod { get; set; }
    public OrderStatus Status { get; set; }
    public decimal ItemsTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public string? PromoCode { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal GrandTotal { get; set; }
    public string PaymentToken { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public IReadOnlyList<OrderItemDto> Items { get; set; } = Array.Empty<OrderItemDto>();
}
