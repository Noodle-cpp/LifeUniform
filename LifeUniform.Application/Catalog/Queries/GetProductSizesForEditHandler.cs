using MediatR;
using LifeUniform.Domain.Catalog;
using LifeUniform.Application.Catalog.Dto;

namespace LifeUniform.Application.Catalog.Queries;

public class GetProductSizesForEditHandler : IRequestHandler<GetProductSizesForEditQuery, ProductSizeEditDto>
{
    private readonly ICatalogRepository _catalogRepository;

    public GetProductSizesForEditHandler(ICatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public async Task<ProductSizeEditDto> Handle(GetProductSizesForEditQuery request, CancellationToken cancellationToken)
    {
        var product = await _catalogRepository.GetProductWithSizesBySlugAsync(request.Slug, cancellationToken);
        if (product is null)
            throw new KeyNotFoundException($"Product not found: {request.Slug}");

        var allSizes = await _catalogRepository.GetAllSizesAsync(cancellationToken);
        var selectedSizeIds = product.SizeOptions.Select(so => so.SizeId).ToHashSet();

        return new ProductSizeEditDto
        {
            ProductId = product.Id,
            Name = product.Name,
            Slug = product.Slug,
            Sizes = allSizes.Select(s => new ProductSizeItemDto
            {
                SizeId = s.Id,
                Label = s.Label,
                IsSelected = selectedSizeIds.Contains(s.Id)
            }).ToList()
        };
    }
}

