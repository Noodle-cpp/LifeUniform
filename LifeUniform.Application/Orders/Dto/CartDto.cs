namespace LifeUniform.Application.Orders.Dto;

public class CartItemDto
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
    public decimal LineTotal => UnitPrice * Quantity;
}

public class CartDto
{
    public IReadOnlyList<CartItemDto> Items { get; set; } = Array.Empty<CartItemDto>();
    public decimal ItemsTotal { get; set; }
    public int ItemCount { get; set; }
}
