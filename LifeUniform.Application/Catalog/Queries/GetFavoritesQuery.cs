using LifeUniform.Application.Catalog.Dto;
using LifeUniform.Application.Catalog.Mapping;
using LifeUniform.Domain.Catalog;
using MediatR;

namespace LifeUniform.Application.Catalog.Queries;

public class GetFavoritesQuery : IRequest<IReadOnlyList<ProductCardDto>>
{
    public string UserId { get; init; } = string.Empty;
    public IReadOnlyList<Guid>? ProductIds { get; init; }
}

public class GetFavoritesHandler : IRequestHandler<GetFavoritesQuery, IReadOnlyList<ProductCardDto>>
{
    private readonly ICatalogRepository _catalog;

    public GetFavoritesHandler(ICatalogRepository catalog) => _catalog = catalog;

    public async Task<IReadOnlyList<ProductCardDto>> Handle(GetFavoritesQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Guid> ids;
        if (request.ProductIds is not null)
        {
            ids = request.ProductIds;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
                return Array.Empty<ProductCardDto>();

            ids = (await _catalog.GetFavoriteProductIdsAsync(request.UserId, cancellationToken)).ToList();
        }

        if (ids.Count == 0)
            return Array.Empty<ProductCardDto>();

        var products = await _catalog.GetProductsByIdsAsync(ids.ToList(), cancellationToken);
        return CatalogMapper.ToProductCards(products)
            .Select(p =>
            {
                p.IsFavorite = true;
                return p;
            })
            .ToList();
    }
}
