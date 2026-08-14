using System.Security.Claims;
using LifeUniform.Application.Catalog.Dto;
using LifeUniform.Application.Catalog.Mapping;
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

    [BindProperty(SupportsGet = true, Name = "categoryId")]
    public List<Guid> CategoryIds { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true, Name = "color")]
    public List<string> Colors { get; set; } = new();

    [BindProperty(SupportsGet = true, Name = "page")]
    public int PageNumber { get; set; } = 1;

    public CatalogProductsPageDto Vm { get; private set; } = new();

    public async Task OnGetAsync()
    {
        ProductGender? gender = null;
        if (Gender is int g && Enum.IsDefined(typeof(ProductGender), g))
            gender = (ProductGender)g;

        var page = PageNumber < 1 ? 1 : PageNumber;
        CategoryIds = CategoryIds.Where(id => id != Guid.Empty).Distinct().ToList();
        Colors = Colors
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Select(c => c.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        Vm = await _mediator.Send(new GetCatalogProductsQuery
        {
            Gender = gender,
            CategoryIds = CategoryIds,
            Search = Search,
            Colors = Colors,
            Page = page,
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        });

        if (string.IsNullOrWhiteSpace(User.FindFirstValue(ClaimTypes.NameIdentifier)))
            _favorites.ApplyGuest(Vm.Items);
    }

    public bool IsColorSelected(string colorName) =>
        Colors.Any(c => string.Equals(c, colorName, StringComparison.OrdinalIgnoreCase));

    public bool IsCategorySelected(Guid categoryId) =>
        CategoryIds.Contains(categoryId);

    public IReadOnlyList<CategoryFilterGroupDto> CategoryGroups =>
        CatalogMapper.GroupCategoriesByGender(Vm.Categories);

    public string ToggleColorUrl(string colorName)
    {
        var next = Colors
            .Where(c => !string.Equals(c, colorName, StringComparison.OrdinalIgnoreCase))
            .ToList();
        if (!IsColorSelected(colorName))
            next.Add(colorName);
        return BuildUrl(CategoryIds, next, page: 1);
    }

    public string ToggleCategoryUrl(Guid categoryId)
    {
        var next = CategoryIds.Where(id => id != categoryId).ToList();
        if (!IsCategorySelected(categoryId))
            next.Add(categoryId);
        return BuildUrl(next, Colors, page: 1);
    }

    public string ClearCategoriesUrl() => BuildUrl(Array.Empty<Guid>(), Colors, page: 1);

    public string PagerUrl(int page) => BuildUrl(CategoryIds, Colors, page);

    private string BuildUrl(IEnumerable<Guid> categoryIds, IEnumerable<string> colors, int page)
    {
        var parts = new List<string>();
        if (Gender is int g)
            parts.Add($"gender={g}");
        foreach (var id in categoryIds.Distinct())
            parts.Add($"categoryId={id}");
        foreach (var color in colors.Where(c => !string.IsNullOrWhiteSpace(c)).Distinct(StringComparer.OrdinalIgnoreCase))
            parts.Add($"color={Uri.EscapeDataString(color)}");
        if (!string.IsNullOrWhiteSpace(Search))
            parts.Add($"search={Uri.EscapeDataString(Search)}");
        if (page > 1)
            parts.Add($"page={page}");

        var path = Url.Page("/Catalog/Index") ?? "/Catalog";
        return parts.Count == 0 ? path : $"{path}?{string.Join("&", parts)}";
    }
}
