namespace LifeUniform.Application.Catalog.Dto;

public class SizeCardDto
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    /// <summary>When set on PDP, whether this size is in stock for the active color context.</summary>
    public bool IsInStock { get; set; } = true;
}

public class ColorSizeStockDto
{
    public string ColorName { get; set; } = string.Empty;
    public Guid SizeId { get; set; }
    public bool IsInStock { get; set; }
}

