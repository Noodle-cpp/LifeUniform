namespace LifeUniform.Domain.Catalog;

public class Product
{
    public Guid Id { get; set; }
    public ProductGender Gender { get; set; }

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    /// <summary>Короткое имя коллекции/модели для крупного заголовка (например AURA).</summary>
    public string? ShortName { get; set; }
    public string Slug { get; set; } = string.Empty;
    /// <summary>Артикул (SKU), например LU-AURA-001.</summary>
    public string? Sku { get; set; }
    public string? Description { get; set; }
    /// <summary>Состав / материал (например, «65% хлопок, 35% полиэстер»).</summary>
    public string? Material { get; set; }
    /// <summary>Инструкции по уходу (многострочный текст).</summary>
    public string? CareInstructions { get; set; }

    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsInStock { get; set; } = true;

    /// <summary>Порог бесплатной доставки для подсказки на карточке (null = 8000 по умолчанию в UI).</summary>
    public decimal? FreeShippingFrom { get; set; }

    // “Популярность” для MVP витрины
    public int PopularityRank { get; set; }

    public List<ProductImage> Images { get; set; } = new();
    public List<ProductSizeOption> SizeOptions { get; set; } = new();
    public List<ProductColorOption> ColorOptions { get; set; } = new();
    public List<ProductColorSizeStock> ColorSizeStocks { get; set; } = new();
    public string? SizeChartImageUrl { get; set; }

    public ProductImage? PrimaryImage
        => Images
            .Where(i => i.IsPrimary)
            .OrderBy(i => i.SortOrder)
            .FirstOrDefault()
           ?? Images.OrderBy(i => i.SortOrder).FirstOrDefault();
}

