using LifeUniform.Application.Abstractions.Payment;
using MediatR;

namespace LifeUniform.Application.Orders.Commands;

public class ConfirmPaymentCommand : IRequest
{
    public string PaymentToken { get; init; } = string.Empty;
}

public class ConfirmPaymentHandler : IRequestHandler<ConfirmPaymentCommand>
{
    private readonly IPaymentFacade _payments;

    public ConfirmPaymentHandler(IPaymentFacade payments) => _payments = payments;

    public Task Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken) =>
        _payments.ConfirmStubPaidAsync(request.PaymentToken, cancellationToken);
}
