namespace LifeUniform.Domain.Orders;

public class Order
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;

    public string? UserId { get; set; }

    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;

    public DeliveryMethod DeliveryMethod { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.PendingPayment;

    public decimal ItemsTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public string? PromoCode { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GrandTotal { get; set; }


    /// <summary>Заглушка QR/SБП: токен платежа для страницы оплаты.</summary>
    public string PaymentToken { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? PaidAt { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}
