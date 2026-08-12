using MediatR;
using LifeUniform.Application.Catalog.Dto;

namespace LifeUniform.Application.Catalog.Queries;

public class GetCategoriesForSelectQuery : IRequest<IReadOnlyList<CategorySelectDto>>
{
}

