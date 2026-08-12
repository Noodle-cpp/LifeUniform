using LifeUniform.Domain.Orders;
using MediatR;

namespace LifeUniform.Application.Orders.Commands;

public class UpdateOrderStatusCommand : IRequest
{
    public Guid OrderId { get; init; }
    public OrderStatus Status { get; init; }
}

public class UpdateOrderStatusHandler : IRequestHandler<UpdateOrderStatusCommand>
{
    private readonly IOrderRepository _orders;

    public UpdateOrderStatusHandler(IOrderRepository orders) => _orders = orders;

    public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        await _orders.UpdateStatusAsync(request.OrderId, request.Status, cancellationToken);
    }
}
