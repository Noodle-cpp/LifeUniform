using LifeUniform.Application.Catalog.Dto;
using LifeUniform.Application.Catalog.Mapping;
using LifeUniform.Domain.Catalog;
using MediatR;

namespace LifeUniform.Application.Catalog.Queries;

public class GetProductDetailsHandler : IRequestHandler<GetProductDetailsQuery, ProductDetailsDto>
{
    private readonly ICatalogRepository _catalogRepository;

    public GetProductDetailsHandler(ICatalogRepository catalogRepository) => _catalogRepository = catalogRepository;

    public async Task<ProductDetailsDto> Handle(GetProductDetailsQuery request, CancellationToken cancellationToken)
    {
        var product = await _catalogRepository.GetProductBySlugAsync(request.Slug, cancellationToken);
        if (product is null)
            throw new KeyNotFoundException($"Product not found: {request.Slug}");

        var dto = CatalogMapper.ToProductDetails(product);

        var related = await _catalogRepository.GetRelatedProductsAsync(
            product.Id,
            product.CategoryId,
            4,
            cancellationToken);
        dto.RelatedProducts = CatalogMapper.ToProductCards(related);

        if (!string.IsNullOrWhiteSpace(request.UserId))
            dto.IsFavorite = await _catalogRepository.IsProductFavoriteAsync(request.UserId!, product.Id, cancellationToken);

        return dto;
    }
}
