namespace LifeUniform.Application.Catalog.Dto;

public class CategoryAdminCardDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int Gender { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}

