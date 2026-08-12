using MediatR;
using LifeUniform.Application.Catalog.Dto;

namespace LifeUniform.Application.Catalog.Queries;

public class GetCategoryForEditQuery : IRequest<CategoryEditDto?>
{
    public string Slug { get; init; } = string.Empty;
}

