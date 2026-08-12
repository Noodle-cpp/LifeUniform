using MediatR;
using LifeUniform.Application.Catalog.Dto;

namespace LifeUniform.Application.Catalog.Queries;

public class GetProductSizesForEditQuery : IRequest<ProductSizeEditDto>
{
    public string Slug { get; init; } = string.Empty;
}

