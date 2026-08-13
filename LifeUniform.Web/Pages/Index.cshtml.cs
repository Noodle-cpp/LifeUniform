using Microsoft.AspNetCore.Mvc.RazorPages;
using LifeUniform.Application.Catalog.Dto;
using LifeUniform.Application.Catalog.Queries;
using LifeUniform.Web.Services;
using MediatR;
using System.Security.Claims;

namespace LifeUniform.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly IFavoriteState _favorites;

        public CatalogHomeDto Vm { get; private set; } = new();

        public IndexModel(IMediator mediator, IFavoriteState favorites)
        {
            _mediator = mediator;
            _favorites = favorites;
        }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Vm = await _mediator.Send(new GetCatalogHomeQuery
            {
                UserId = userId
            });

            if (string.IsNullOrWhiteSpace(userId))
            {
                _favorites.ApplyGuest(Vm.PopularProducts);
                _favorites.ApplyGuest(Vm.WomenProducts);
                _favorites.ApplyGuest(Vm.MenProducts);
            }
        }
    }
}
