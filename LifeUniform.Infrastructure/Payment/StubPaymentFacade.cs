using LifeUniform.Application.Abstractions.Payment;
using LifeUniform.Domain.Orders;
using Microsoft.Extensions.Logging;

namespace LifeUniform.Infrastructure.Payment;

public class StubPaymentFacade : IPaymentFacade
{
    private readonly IOrderRepository _orders;
    private readonly ILogger<StubPaymentFacade> _logger;

    public StubPaymentFacade(IOrderRepository orders, ILogger<StubPaymentFacade> logger)
    {
        _orders = orders;
        _logger = logger;
    }

    public Task<PaymentSession> CreateSessionAsync(CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        var qr = $"STUB-SBP:{request.OrderNumber}:{request.Amount:0.00}:{request.PaymentToken}";
        _logger.LogInformation("Created stub payment session for {OrderNumber}", request.OrderNumber);
        return Task.FromResult(new PaymentSession(
            request.PaymentToken,
            qr,
            "Оплата через СБП/QR (заглушка). Нажмите «Я оплатил» после эмуляции."));
    }

    public async Task ConfirmStubPaidAsync(string paymentToken, CancellationToken cancellationToken)
    {
        var order = await _orders.GetByPaymentTokenAsync(paymentToken, cancellationToken)
            ?? throw new KeyNotFoundException("Заказ не найден.");

        if (order.Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Заказ отменён.");

        if (order.Status == OrderStatus.PendingPayment)
            await _orders.UpdateStatusAsync(order.Id, OrderStatus.Paid, cancellationToken);

        _logger.LogInformation("Stub payment confirmed for token {Token}", paymentToken);
    }
}
