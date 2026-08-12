namespace LifeUniform.Domain.Catalog;

public class ProductImage
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string FileNameOriginal { get; set; } = string.Empty;
    public string FileNamePreview { get; set; } = string.Empty;
    public string FileNameWebp { get; set; } = string.Empty;

    public string AltText { get; set; } = string.Empty;

    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }
}

