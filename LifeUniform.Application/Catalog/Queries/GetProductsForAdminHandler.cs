using LifeUniform.Application.Catalog.Dto;
using LifeUniform.Application.Catalog.Mapping;
using LifeUniform.Domain.Catalog;
using MediatR;

namespace LifeUniform.Application.Catalog.Queries;

public class GetProductsForAdminHandler : IRequestHandler<GetProductsForAdminQuery, IReadOnlyList<ProductAdminCardDto>>
{
    private readonly ICatalogRepository _catalogRepository;

    public GetProductsForAdminHandler(ICatalogRepository catalogRepository) => _catalogRepository = catalogRepository;

    public async Task<IReadOnlyList<ProductAdminCardDto>> Handle(GetProductsForAdminQuery request, CancellationToken cancellationToken)
    {
        var products = await _catalogRepository.GetProductsForAdminAsync(cancellationToken);
        return products.Select(CatalogMapper.ToProductAdminCard).ToList();
    }
}
