using LifeUniform.Application.Orders.Commands;
using LifeUniform.Application.Orders.Dto;
using LifeUniform.Domain.Orders;
using MediatR;

namespace LifeUniform.Application.Orders.Queries;

public class GetOrdersForUserQuery : IRequest<IReadOnlyList<OrderDto>>
{
    public string UserId { get; init; } = string.Empty;
}

public class GetOrdersForUserHandler : IRequestHandler<GetOrdersForUserQuery, IReadOnlyList<OrderDto>>
{
    private readonly IOrderRepository _orders;

    public GetOrdersForUserHandler(IOrderRepository orders) => _orders = orders;

    public async Task<IReadOnlyList<OrderDto>> Handle(GetOrdersForUserQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
            return Array.Empty<OrderDto>();

        var list = await _orders.GetByUserIdAsync(request.UserId, cancellationToken);
        return list.Select(CreateOrderHandler.Map).ToList();
    }
}
