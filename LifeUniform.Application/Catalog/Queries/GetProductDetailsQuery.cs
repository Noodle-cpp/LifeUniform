using MediatR;
using LifeUniform.Application.Catalog.Dto;

namespace LifeUniform.Application.Catalog.Queries;

public class GetProductDetailsQuery : IRequest<ProductDetailsDto>
{
    public string Slug { get; init; } = string.Empty;
    public string? UserId { get; init; }
}

