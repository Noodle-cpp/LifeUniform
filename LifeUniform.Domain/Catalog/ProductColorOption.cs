namespace LifeUniform.Domain.Catalog;

public class ProductColorOption
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string Hex { get; set; } = "#cccccc";
    public string? ImageFileName { get; set; }
    public int SortOrder { get; set; }
}
