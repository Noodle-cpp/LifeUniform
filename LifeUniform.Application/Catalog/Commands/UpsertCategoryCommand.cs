using MediatR;
using LifeUniform.Domain.Catalog;

namespace LifeUniform.Application.Catalog.Commands;

public class UpsertCategoryCommand : IRequest
{
    public string Slug { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public ProductGender Gender { get; init; }
    public string? Description { get; init; }
    public int SortOrder { get; init; }
    public bool IsActive { get; init; }
}

