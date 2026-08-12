using LifeUniform.Application.Catalog.Commands;
using LifeUniform.Application.Catalog.Dto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Areas.Admin.Pages.Catalog.Categories;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly IMediator _mediator;

    [BindProperty]
    public CategoryEditDto Vm { get; set; } = new();

    public CreateModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public void OnGet()
    {
        Vm.Gender = 1;
        Vm.SortOrder = 100;
        Vm.IsActive = true;
    }

    public async Task<IActionResult> OnPostAsync()
    {
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

