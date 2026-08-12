using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LifeUniform.Application.Catalog.Dto;
using LifeUniform.Application.Catalog.Queries;
using MediatR;
using System.Security.Claims;

namespace LifeUniform.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;

        public CatalogHomeDto Vm { get; private set; } = new();

        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Vm = await _mediator.Send(new GetCatalogHomeQuery
            {
                UserId = userId
            });
        }
    }
}
