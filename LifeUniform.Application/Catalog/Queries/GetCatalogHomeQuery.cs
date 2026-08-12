using MediatR;
using LifeUniform.Domain.Catalog;
using LifeUniform.Application.Catalog.Dto;

namespace LifeUniform.Application.Catalog.Queries;

public class GetCatalogHomeQuery : IRequest<CatalogHomeDto>
{
    public ProductGender? Gender { get; init; }
    public string? UserId { get; init; }
}

