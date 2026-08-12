using LifeUniform.Application.Orders.Dto;
using LifeUniform.Domain.Cart;
using MediatR;

namespace LifeUniform.Application.Orders.Queries;

public class GetCartQuery : IRequest<CartDto>;

public class GetCartHandler : IRequestHandler<GetCartQuery, CartDto>
{
    private readonly ICartService _cart;

    public GetCartHandler(ICartService cart) => _cart = cart;

    public Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var items = _cart.GetItems()
            .Select(x => new CartItemDto
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                ProductSlug = x.ProductSlug,
                PreviewImageUrl = x.PreviewImageUrl,
                ColorName = x.ColorName,
                SizeId = x.SizeId,
                SizeLabel = x.SizeLabel,
                UnitPrice = x.UnitPrice,
                Quantity = x.Quantity
            })
            .ToList();

        return Task.FromResult(new CartDto
        {
            Items = items,
            ItemsTotal = items.Sum(i => i.LineTotal),
            ItemCount = items.Sum(i => i.Quantity)
        });
    }
}
