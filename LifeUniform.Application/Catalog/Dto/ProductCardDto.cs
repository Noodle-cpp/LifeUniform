namespace LifeUniform.Application.Catalog.Dto;

public class ProductCardDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }

    public string PreviewImageUrl { get; set; } =
        "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='600' height='600' viewBox='0 0 600 600'%3E%3Crect width='100%25' height='100%25' fill='%23f3f4f6'/%3E%3Ctext x='50%25' y='50%25' dominant-baseline='middle' text-anchor='middle' font-family='Arial' font-size='28' fill='%239ca3af'%3ENo image%3C/text%3E%3C/svg%3E";

    public IReadOnlyList<ProductColorDto> Colors { get; set; } = Array.Empty<ProductColorDto>();

    public bool IsFavorite { get; set; }
}

