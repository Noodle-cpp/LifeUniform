namespace LifeUniform.Domain.Orders;

public interface IOrderRepository
{
    Task<Order> CreateAsync(Order order, CancellationToken cancellationToken);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Order?> GetByPaymentTokenAsync(string paymentToken, CancellationToken cancellationToken);
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Order>> GetByUserIdAsync(string userId, CancellationToken cancellationToken);
    Task UpdateStatusAsync(Guid orderId, OrderStatus status, CancellationToken cancellationToken);
}

