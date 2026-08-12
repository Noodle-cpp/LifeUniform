using LifeUniform.Domain.Cart;
using MediatR;

namespace LifeUniform.Application.Orders.Commands;

public class UpdateCartItemCommand : IRequest
{
    public Guid ProductId { get; init; }
    public Guid SizeId { get; init; }
    public string? ColorName { get; init; }
    public int Quantity { get; init; }
}

public class UpdateCartItemHandler : IRequestHandler<UpdateCartItemCommand>
{
    private readonly ICartService _cart;

    public UpdateCartItemHandler(ICartService cart) => _cart = cart;

    public Task Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        _cart.UpdateQuantity(request.ProductId, request.SizeId, request.Quantity, request.ColorName);
        return Task.CompletedTask;
    }
}

public class RemoveCartItemCommand : IRequest
{
    public Guid ProductId { get; init; }
    public Guid SizeId { get; init; }
    public string? ColorName { get; init; }
}

public class RemoveCartItemHandler : IRequestHandler<RemoveCartItemCommand>
{
    private readonly ICartService _cart;

    public RemoveCartItemHandler(ICartService cart) => _cart = cart;

    public Task Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        _cart.Remove(request.ProductId, request.SizeId, request.ColorName);
        return Task.CompletedTask;
    }
}

