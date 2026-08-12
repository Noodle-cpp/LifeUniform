using MediatR;
using LifeUniform.Application.Catalog.Dto;

namespace LifeUniform.Application.Catalog.Queries;

public class GetProductForAdminEditQuery : IRequest<ProductEditDto?>
{
    public string Slug { get; init; } = string.Empty;
}

