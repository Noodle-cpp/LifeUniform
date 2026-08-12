namespace LifeUniform.Application.Catalog.Dto;

public class ProductColorEditDto
{
    public string Name { get; set; } = string.Empty;
    public string Hex { get; set; } = "#cccccc";
    public string? ImageUrl { get; set; }
}

public class ProductEditDto
{
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string? Sku { get; set; }

    public int Gender { get; set; }
    public Guid CategoryId { get; set; }

    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }

    public string? Description { get; set; }
    public string? Material { get; set; }
    public string? CareInstructions { get; set; }
    public string? SizeChartImageUrl { get; set; } = "/images/size-chart.svg";
    public bool IsInStock { get; set; } = true;
    public decimal? FreeShippingFrom { get; set; }
    public int PopularityRank { get; set; }

    public IReadOnlyList<CategorySelectDto> Categories { get; set; } = Array.Empty<CategorySelectDto>();
    public IReadOnlyList<ProductSizeItemDto> Sizes { get; set; } = Array.Empty<ProductSizeItemDto>();
    public List<ProductColorEditDto> Colors { get; set; } = new();

    /// <summary>Keys "ColorName||SizeId" that are in stock.</summary>
    public List<string> InStockColorSizeKeys { get; set; } = new();

    public List<ProductImageEditDto> Images { get; set; } = new();

    public string PreviewImageUrl { get; set; } = "/images/placeholder-product.svg";
}

public class ProductImageEditDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }
}
