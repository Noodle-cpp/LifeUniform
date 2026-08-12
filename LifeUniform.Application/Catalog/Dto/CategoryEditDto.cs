namespace LifeUniform.Application.Catalog.Dto;

public class CategoryEditDto
{
    public string Slug { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public int Gender { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

