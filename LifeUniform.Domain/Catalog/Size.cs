namespace LifeUniform.Domain.Catalog;

public class Size
{
    public Guid Id { get; set; }

    // Например: S, M, L, XL, 2XL ... 5XL
    public string Label { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;
}

