using LifeUniform.Application.Catalog.Dto;
using LifeUniform.Application.Catalog.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Areas.Admin.Pages.Catalog
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;

        public IReadOnlyList<ProductAdminCardDto> Vm { get; private set; } = Array.Empty<ProductAdminCardDto>();

        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task OnGetAsync()
        {
            Vm = await _mediator.Send(new GetProductsForAdminQuery());
        }
    }
}

