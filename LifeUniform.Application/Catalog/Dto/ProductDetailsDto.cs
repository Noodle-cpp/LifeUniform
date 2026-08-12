namespace LifeUniform.Application.Catalog.Dto;

public class ProductDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string? Sku { get; set; }

    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;
    public int Gender { get; set; }

    public string? Description { get; set; }
    public string? Material { get; set; }
    public string? CareInstructions { get; set; }

    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int? DiscountPercent =>
        DiscountPrice is decimal d && Price > 0
            ? (int)Math.Round((1 - d / Price) * 100)
            : null;
    public decimal? SavingsAmount =>
        DiscountPrice is decimal d && d < Price ? Price - d : null;

    public bool IsInStock { get; set; } = true;
    public decimal FreeShippingFrom { get; set; } = 8000m;

    public string PreviewImageUrl { get; set; } = string.Empty;
    public string SizeChartImageUrl { get; set; } = "/images/size-chart.svg";
    public IReadOnlyList<ProductColorDto> Colors { get; set; } = Array.Empty<ProductColorDto>();
    public IReadOnlyList<string> GalleryImageUrls { get; set; } = Array.Empty<string>();
    public IReadOnlyList<SizeCardDto> Sizes { get; set; } = Array.Empty<SizeCardDto>();
    /// <summary>In-stock size IDs per color name (for filtering sizes on PDP).</summary>
    public IReadOnlyList<ColorSizeStockDto> ColorSizeStocks { get; set; } = Array.Empty<ColorSizeStockDto>();
    public IReadOnlyList<ProductCardDto> RelatedProducts { get; set; } = Array.Empty<ProductCardDto>();

    public bool IsFavorite { get; set; }
}
