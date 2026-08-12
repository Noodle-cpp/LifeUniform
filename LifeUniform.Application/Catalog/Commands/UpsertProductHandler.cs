using MediatR;
using LifeUniform.Application.Abstractions.Caching;
using LifeUniform.Domain.Catalog;

namespace LifeUniform.Application.Catalog.Commands;

public class UpsertProductHandler : IRequestHandler<UpsertProductCommand>
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly ICatalogCacheInvalidator _cacheInvalidator;

    public UpsertProductHandler(ICatalogRepository catalogRepository, ICatalogCacheInvalidator cacheInvalidator)
    {
        _catalogRepository = catalogRepository;
        _cacheInvalidator = cacheInvalidator;
    }

    public async Task Handle(UpsertProductCommand request, CancellationToken cancellationToken)
    {
        await _catalogRepository.UpsertProductAsync(
            slug: request.Slug,
            gender: request.Gender,
            categoryId: request.CategoryId,
            price: request.Price,
            discountPrice: request.DiscountPrice,
            name: request.Name,
            shortName: request.ShortName,
            sku: request.Sku,
            description: request.Description,
            material: request.Material,
            careInstructions: request.CareInstructions,
            sizeChartImageUrl: request.SizeChartImageUrl,
            isInStock: request.IsInStock,
            freeShippingFrom: request.FreeShippingFrom,
            popularityRank: request.PopularityRank,
            sizeIds: request.SizeIds,
            colors: request.Colors.Select(c => (c.Name, c.Hex, c.ImageUrl)).ToList(),
            colorSizeStocks: request.ColorSizeStocks
                .Select(s => (s.ColorName, s.SizeId, s.IsInStock))
                .ToList(),
            imageFileNamesToAdd: request.ImageFileNamesToAdd,
            imageAltText: request.ImageAltText,
            cancellationToken: cancellationToken);

        _cacheInvalidator.InvalidateCatalog();
    }
}

