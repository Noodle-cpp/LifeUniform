using LifeUniform.Application.Catalog.Dto;
using LifeUniform.Application.Catalog.Queries;
using LifeUniform.Web.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Pages.Favorites;

public class IndexModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IFavoriteState _favorites;

    public IndexModel(IMediator mediator, IFavoriteState favorites)
    {
        _mediator = mediator;
        _favorites = favorites;
    }

    public IReadOnlyList<ProductCardDto> Vm { get; private set; } = Array.Empty<ProductCardDto>();

    public async Task OnGetAsync()
    {
        var ids = await _favorites.GetIdsAsync(HttpContext.RequestAborted);
        Vm = await _mediator.Send(new GetFavoritesQuery { ProductIds = ids.ToList() });
    }
}
