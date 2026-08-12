using MediatR;
using LifeUniform.Domain.Catalog;

namespace LifeUniform.Application.Catalog.Commands;

public class ProductColorInput
{
    public string Name { get; init; } = string.Empty;
    public string Hex { get; init; } = "#cccccc";
    public string? ImageUrl { get; init; }
}

public class ProductColorSizeStockInput
{
    public string ColorName { get; init; } = string.Empty;
    public Guid SizeId { get; init; }
    public bool IsInStock { get; init; } = true;
}

public class UpsertProductCommand : IRequest
{
    public string Slug { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? ShortName { get; init; }
    public string? Sku { get; init; }
    public ProductGender Gender { get; init; }
    public Guid CategoryId { get; init; }

    public decimal Price { get; init; }
    public decimal? DiscountPrice { get; init; }

    public string? Description { get; init; }
    public string? Material { get; init; }
    public string? CareInstructions { get; init; }
    public string? SizeChartImageUrl { get; init; }
    public bool IsInStock { get; init; } = true;
    public decimal? FreeShippingFrom { get; init; }
    public int PopularityRank { get; init; }

    public IReadOnlyList<Guid> SizeIds { get; init; } = Array.Empty<Guid>();
    public IReadOnlyList<ProductColorInput> Colors { get; init; } = Array.Empty<ProductColorInput>();
    public IReadOnlyList<ProductColorSizeStockInput> ColorSizeStocks { get; init; } = Array.Empty<ProductColorSizeStockInput>();

    /// <summary>New image file names to append (does not replace existing gallery).</summary>
    public IReadOnlyList<string> ImageFileNamesToAdd { get; init; } = Array.Empty<string>();
    public string? ImageAltText { get; init; }
}

