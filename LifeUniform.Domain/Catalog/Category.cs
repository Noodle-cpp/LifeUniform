namespace LifeUniform.Domain.Catalog;

public class Category
{
    public Guid Id { get; set; }
    public ProductGender Gender { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public List<Product> Products { get; set; } = new();
}

