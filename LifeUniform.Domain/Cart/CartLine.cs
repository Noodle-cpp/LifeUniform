namespace LifeUniform.Domain.Cart;

public class CartLine
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSlug { get; set; } = string.Empty;
    public string PreviewImageUrl { get; set; } = string.Empty;
    public string? ColorName { get; set; }

    public Guid SizeId { get; set; }
    public string SizeLabel { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
