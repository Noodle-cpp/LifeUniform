using System.Security.Claims;
using LifeUniform.Application.Catalog.Queries;
using LifeUniform.Domain.Catalog;
using LifeUniform.Web.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Pages.Catalog;

public class IndexModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IFavoriteState _favorites;

    public IndexModel(IMediator mediator, IFavoriteState favorites)
    {
        _mediator = mediator;
        _favorites = favorites;
    }

    [BindProperty(SupportsGet = true)]
    public int? Gender { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid? CategoryId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Color { get; set; }

    [BindProperty(SupportsGet = true, Name = "page")]
    public int PageNumber { get; set; } = 1;

    public CatalogProductsPageDto Vm { get; private set; } = new();

    public async Task OnGetAsync()
    {
        ProductGender? gender = null;
        if (Gender is int g && Enum.IsDefined(typeof(ProductGender), g))
            gender = (ProductGender)g;

        var page = PageNumber < 1 ? 1 : PageNumber;

        Vm = await _mediator.Send(new GetCatalogProductsQuery
        {
            Gender = gender,
            CategoryId = CategoryId,
            Search = Search,
            Color = Color,
            Page = page,
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        });

        if (string.IsNullOrWhiteSpace(User.FindFirstValue(ClaimTypes.NameIdentifier)))
            _favorites.ApplyGuest(Vm.Items);
    }
}
