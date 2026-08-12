using LifeUniform.Application.Orders.Commands;
using LifeUniform.Application.Orders.Dto;
using LifeUniform.Domain.Orders;
using MediatR;

namespace LifeUniform.Application.Orders.Queries;

public class GetOrderByPaymentTokenQuery : IRequest<OrderDto?>
{
    public string PaymentToken { get; init; } = string.Empty;
}

public class GetOrderByPaymentTokenHandler : IRequestHandler<GetOrderByPaymentTokenQuery, OrderDto?>
{
    private readonly IOrderRepository _orders;

    public GetOrderByPaymentTokenHandler(IOrderRepository orders) => _orders = orders;

    public async Task<OrderDto?> Handle(GetOrderByPaymentTokenQuery request, CancellationToken cancellationToken)
    {
        var order = await _orders.GetByPaymentTokenAsync(request.PaymentToken, cancellationToken);
        return order is null ? null : CreateOrderHandler.Map(order);
    }
}

public class GetOrdersForAdminQuery : IRequest<IReadOnlyList<OrderDto>>;

public class GetOrdersForAdminHandler : IRequestHandler<GetOrdersForAdminQuery, IReadOnlyList<OrderDto>>
{
    private readonly IOrderRepository _orders;

    public GetOrdersForAdminHandler(IOrderRepository orders) => _orders = orders;

    public async Task<IReadOnlyList<OrderDto>> Handle(GetOrdersForAdminQuery request, CancellationToken cancellationToken)
    {
        var list = await _orders.GetAllAsync(cancellationToken);
        return list.Select(CreateOrderHandler.Map).ToList();
    }
}

public class GetOrderByIdQuery : IRequest<OrderDto?>
{
    public Guid Id { get; init; }
}

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IOrderRepository _orders;

    public GetOrderByIdHandler(IOrderRepository orders) => _orders = orders;

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orders.GetByIdAsync(request.Id, cancellationToken);
        return order is null ? null : CreateOrderHandler.Map(order);
    }
}
