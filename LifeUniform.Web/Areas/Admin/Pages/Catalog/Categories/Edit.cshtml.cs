using LifeUniform.Application.Catalog.Commands;
using LifeUniform.Application.Catalog.Dto;
using LifeUniform.Application.Catalog.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Areas.Admin.Pages.Catalog.Categories;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly IMediator _mediator;

    public bool IsNotFound { get; private set; }

    [BindProperty]
    public CategoryEditDto Vm { get; set; } = new();

    public EditModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task OnGetAsync(string slug)
    {
        var dto = await _mediator.Send(new GetCategoryForEditQuery { Slug = slug });
        if (dto is null)
        {
            IsNotFound = true;
            return;
        }

        Vm = dto;
    }

    public async Task<IActionResult> OnPostAsync(string slug)
    {
        if (IsNotFound)
            return Page();

        await _mediator.Send(new UpsertCategoryCommand
        {
            Slug = Vm.Slug,
            Name = Vm.Name,
            Gender = (LifeUniform.Domain.Catalog.ProductGender)Vm.Gender,
            Description = Vm.Description,
            SortOrder = Vm.SortOrder,
            IsActive = Vm.IsActive
        });

        return RedirectToPage("/Catalog/Categories/Index");
    }
}

