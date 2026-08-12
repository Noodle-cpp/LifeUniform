namespace LifeUniform.Application.Abstractions.Payment;

public sealed record CreatePaymentRequest(
    Guid OrderId,
    string OrderNumber,
    decimal Amount,
    string PaymentToken,
    string CustomerEmail);

public sealed record PaymentSession(string PaymentToken, string QrPayload, string DisplayMessage);

public interface IPaymentFacade
{
    Task<PaymentSession> CreateSessionAsync(CreatePaymentRequest request, CancellationToken cancellationToken);
    Task ConfirmStubPaidAsync(string paymentToken, CancellationToken cancellationToken);
}
