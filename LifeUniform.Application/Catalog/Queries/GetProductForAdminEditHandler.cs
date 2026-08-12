using MediatR;
using LifeUniform.Application.Catalog.Dto;
using LifeUniform.Application.Catalog.Mapping;
using LifeUniform.Domain.Catalog;

namespace LifeUniform.Application.Catalog.Queries;

public class GetProductForAdminEditHandler : IRequestHandler<GetProductForAdminEditQuery, ProductEditDto?>
{
    private readonly ICatalogRepository _catalogRepository;

    public GetProductForAdminEditHandler(ICatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public async Task<ProductEditDto?> Handle(GetProductForAdminEditQuery request, CancellationToken cancellationToken)
    {
        var product = await _catalogRepository.GetProductWithImagesAndSizesBySlugAsync(request.Slug, cancellationToken);
        if (product is null)
            return null;

        var categories = await _catalogRepository.GetCategoriesForAdminAsync(cancellationToken);
        var allSizes = await _catalogRepository.GetAllSizesAsync(cancellationToken);

        var selectedSizeIds = product.SizeOptions.Select(so => so.SizeId).ToHashSet();

        return new ProductEditDto
        {
            Slug = product.Slug,
            Name = product.Name,
            ShortName = product.ShortName,
            Sku = product.Sku,
            Gender = (int)product.Gender,
            CategoryId = product.CategoryId,
            Price = product.Price,
            DiscountPrice = product.DiscountPrice,
            Description = product.Description,
            Material = product.Material,
            CareInstructions = product.CareInstructions,
            SizeChartImageUrl = product.SizeChartImageUrl ?? "/images/size-chart.svg",
            IsInStock = product.IsInStock,
            FreeShippingFrom = product.FreeShippingFrom,
            PopularityRank = product.PopularityRank,
            Categories = categories.Select(c => new CategorySelectDto { Id = c.Id, Name = c.Name }).ToList(),
            Sizes = allSizes.Select(s => new ProductSizeItemDto
            {
                SizeId = s.Id,
                Label = s.Label,
                IsSelected = selectedSizeIds.Contains(s.Id)
            }).ToList(),
            Colors = product.ColorOptions
                .OrderBy(c => c.SortOrder)
                .Select(c => new ProductColorEditDto
                {
                    Name = c.Name,
                    Hex = c.Hex,
                    ImageUrl = CatalogMapper.ResolveImageFile(c.ImageFileName)
                })
                .ToList(),
            InStockColorSizeKeys = product.ColorSizeStocks
                .Where(s => s.IsInStock)
                .Select(s => $"{s.ColorName}||{s.SizeId:D}")
                .ToList(),
            Images = product.Images
                .OrderByDescending(i => i.IsPrimary)
                .ThenBy(i => i.SortOrder)
                .Select(i => new ProductImageEditDto
                {
                    Id = i.Id,
                    Url = CatalogMapper.ResolveImageFile(i.FileNamePreview) ?? "/images/placeholder-product.svg",
                    IsPrimary = i.IsPrimary,
                    SortOrder = i.SortOrder
                })
                .ToList(),
            PreviewImageUrl = CatalogMapper.ResolvePreviewUrl(product)
        };
    }
}
