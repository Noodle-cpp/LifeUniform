namespace LifeUniform.Application.Catalog.Dto;

public class ProductSizeEditDto
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public IReadOnlyList<ProductSizeItemDto> Sizes { get; set; } = Array.Empty<ProductSizeItemDto>();
}

public class ProductSizeItemDto
{
    public Guid SizeId { get; set; }
    public string Label { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}

