namespace LifeUniform.Application.Catalog.Dto;

public class CategoryCardDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int Gender { get; set; }
}

public class CategoryFilterGroupDto
{
    public int Gender { get; init; }
    public string Title { get; init; } = string.Empty;
    public IReadOnlyList<CategoryCardDto> Items { get; init; } = Array.Empty<CategoryCardDto>();
}

public class NavMenuDto
{
    public IReadOnlyList<CategoryCardDto> Women { get; init; } = Array.Empty<CategoryCardDto>();
    public IReadOnlyList<CategoryCardDto> Men { get; init; } = Array.Empty<CategoryCardDto>();
}

