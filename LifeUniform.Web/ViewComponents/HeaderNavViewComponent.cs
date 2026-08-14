using LifeUniform.Application.Catalog.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LifeUniform.Web.ViewComponents;

public class HeaderNavViewComponent : ViewComponent
{
    private readonly IMediator _mediator;

    public HeaderNavViewComponent(IMediator mediator) => _mediator = mediator;

    public async Task<IViewComponentResult> InvokeAsync(string? variant = null)
    {
        var menu = await _mediator.Send(new GetNavCategoriesQuery());
        var view = string.Equals(variant, "Mobile", StringComparison.OrdinalIgnoreCase)
            ? "Mobile"
            : "Default";
        return View(view, menu);
    }
}
