namespace LifeUniform.Domain.Catalog;

public class ProductSizeOption
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Guid SizeId { get; set; }
    public Size Size { get; set; } = null!;
}

