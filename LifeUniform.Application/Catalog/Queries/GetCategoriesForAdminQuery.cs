using MediatR;
using LifeUniform.Application.Catalog.Dto;

namespace LifeUniform.Application.Catalog.Queries;

public class GetCategoriesForAdminQuery : IRequest<IReadOnlyList<CategoryAdminCardDto>>
{
}

