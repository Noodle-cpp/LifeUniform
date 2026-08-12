namespace LifeUniform.Domain.Catalog;

/// <summary>
/// Availability of a size within a specific color for a product.
/// </summary>
public class ProductColorSizeStock
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>Color name as stored on ProductColorOption (case-insensitive match).</summary>
    public string ColorName { get; set; } = string.Empty;

    public Guid SizeId { get; set; }
    public Size Size { get; set; } = null!;

    public bool IsInStock { get; set; } = true;
}

