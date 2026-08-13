using LifeUniform.Domain.Catalog;
using LifeUniform.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Pages.Favorites;

public class ToggleModel : PageModel
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly IFavoriteState _favorites;

    public ToggleModel(ICatalogRepository catalogRepository, IFavoriteState favorites)
    {
        _catalogRepository = catalogRepository;
        _favorites = favorites;
    }

    [BindProperty]
    public string Slug { get; set; } = string.Empty;

    [BindProperty]
    public Guid? ProductId { get; set; }

    public async Task<IActionResult> OnPostAsync(string slug)
    {
        Slug = slug;
        var productId = ProductId;
        if (productId is null || productId == Guid.Empty)
            productId = await _catalogRepository.GetProductIdBySlugAsync(Slug, HttpContext.RequestAborted);

        if (productId is null)
        {
            if (WantsJson())
                return new JsonResult(new { ok = false, notFound = true }) { StatusCode = 404 };
            return RedirectToPage("/Catalog/Index");
        }

        var isFavorite = await _favorites.ToggleAsync(productId.Value, HttpContext.RequestAborted);

        if (WantsJson())
            return new JsonResult(new { ok = true, isFavorite, slug = Slug });

        var referer = Request.Headers.Referer.ToString();
        if (!string.IsNullOrWhiteSpace(referer) && Uri.TryCreate(referer, UriKind.Absolute, out var uri)
            && string.Equals(uri.Host, Request.Host.Host, StringComparison.OrdinalIgnoreCase))
        {
            return Redirect(referer);
        }

        return RedirectToPage("/Catalog/Details", new { slug });
    }

    private bool WantsJson()
    {
        var accept = Request.Headers.Accept.ToString();
        if (accept.Contains("application/json", StringComparison.OrdinalIgnoreCase))
            return true;
        return string.Equals(
            Request.Headers["X-Requested-With"].ToString(),
            "XMLHttpRequest",
            StringComparison.OrdinalIgnoreCase);
    }
}
