using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Pages.Favorites;

public class ToggleModel : PageModel
{
    private readonly LifeUniform.Domain.Catalog.ICatalogRepository _catalogRepository;

    public ToggleModel(LifeUniform.Domain.Catalog.ICatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    [BindProperty]
    public string Slug { get; set; } = string.Empty;

    public async Task<IActionResult> OnPostAsync(string slug)
    {
        Slug = slug;
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
        {
            if (WantsJson())
                return new JsonResult(new { ok = false, requiresAuth = true }) { StatusCode = 401 };
            return Challenge();
        }

        var product = await _catalogRepository.GetProductBySlugAsync(Slug, HttpContext.RequestAborted);
        if (product is null)
        {
            if (WantsJson())
                return new JsonResult(new { ok = false, notFound = true }) { StatusCode = 404 };
            return RedirectToPage("/Catalog/Index");
        }

        var isFavorite = await _catalogRepository.ToggleProductFavoriteAsync(
            userId, product.Id, HttpContext.RequestAborted);

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

