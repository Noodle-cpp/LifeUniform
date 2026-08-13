using LifeUniform.Application.Catalog.Queries;
using LifeUniform.Domain.Catalog;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Pages.Catalog;

public class SearchModel : PageModel
{
    private readonly IMediator _mediator;

    public SearchModel(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> OnGetAsync(string? q, int pageNumber = 1)
    {
        var result = await _mediator.Send(new GetCatalogProductsQuery
        {
            Search = q,
            Page = pageNumber,
            PageSize = 8
        });

        return new JsonResult(new
        {
            query = q ?? string.Empty,
            totalCount = result.TotalCount,
            page = result.Page,
            totalPages = result.TotalPages,
            items = result.Items.Select(p =>
            {
                var hasDiscount = p.DiscountPrice is decimal d && d > 0 && d < p.Price;
                var discountPercent = hasDiscount
                    ? (int)Math.Round((1 - p.DiscountPrice!.Value / p.Price) * 100)
                    : (int?)null;

                return new
                {
                    name = p.Name,
                    slug = p.Slug,
                    price = p.DiscountPrice ?? p.Price,
                    oldPrice = hasDiscount ? p.Price : (decimal?)null,
                    discountPercent,
                    previewImageUrl = p.PreviewImageUrl,
                    snippet = !string.IsNullOrWhiteSpace(p.CategoryName) ? p.CategoryName : p.Snippet
                };
            })
        });
    }
}
