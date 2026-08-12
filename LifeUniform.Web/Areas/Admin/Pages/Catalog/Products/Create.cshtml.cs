using LifeUniform.Application.Abstractions.Images;
using LifeUniform.Application.Catalog.Commands;
using LifeUniform.Application.Catalog.Dto;
using LifeUniform.Application.Catalog.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LifeUniform.Web.Areas.Admin.Pages.Catalog.Products;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IImageStorage _imageStorage;

    public CreateModel(IMediator mediator, IImageStorage imageStorage)
    {
        _mediator = mediator;
        _imageStorage = imageStorage;
    }

    [BindProperty]
    public ProductEditDto Vm { get; set; } = new();

    public IReadOnlyList<CategorySelectDto> Categories { get; private set; } = Array.Empty<CategorySelectDto>();
    public IReadOnlyList<SizeCardDto> Sizes { get; private set; } = Array.Empty<SizeCardDto>();

    [BindProperty]
    public List<Guid> SelectedSizeIds { get; set; } = new();

    [BindProperty]
    public List<IFormFile> ImageFiles { get; set; } = new();

    [BindProperty]
    public List<string> InStockColorSizeKeys { get; set; } = new();

    public async Task OnGetAsync()
    {
        Vm.Gender = (int)LifeUniform.Domain.Catalog.ProductGender.Women;
        Vm.PopularityRank = 50;
        Vm.Price = 0;
        Vm.IsInStock = true;
        Vm.SizeChartImageUrl = "/images/size-chart.svg";
        Vm.Colors =
        [
            new ProductColorEditDto { Name = "Сиреневый", Hex = "#c4a4c8" },
            new ProductColorEditDto { Name = "Шалфей", Hex = "#8a9a7b" },
            new ProductColorEditDto { Name = "Графит", Hex = "#4b5563" }
        ];

        Categories = await _mediator.Send(new GetCategoriesForSelectQuery());
        Sizes = await _mediator.Send(new GetSizesForSelectQuery());
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Categories = await _mediator.Send(new GetCategoriesForSelectQuery());
        Sizes = await _mediator.Send(new GetSizesForSelectQuery());

        var imageFileNames = await SaveImageFilesAsync(ImageFiles, Vm.Slug);
        await _mediator.Send(BuildCommand(imageFileNames));
        return RedirectToPage("/Catalog/Index", new { area = "Admin" });
    }

    private UpsertProductCommand BuildCommand(IReadOnlyList<string> imageFileNames) => new()
    {
        Slug = Vm.Slug,
        Name = Vm.Name,
        ShortName = Vm.ShortName,
        Sku = Vm.Sku,
        Gender = (LifeUniform.Domain.Catalog.ProductGender)Vm.Gender,
        CategoryId = Vm.CategoryId,
        Price = Vm.Price,
        DiscountPrice = Vm.DiscountPrice,
        Description = Vm.Description,
        Material = Vm.Material,
        CareInstructions = Vm.CareInstructions,
        SizeChartImageUrl = Vm.SizeChartImageUrl,
        IsInStock = Vm.IsInStock,
        FreeShippingFrom = Vm.FreeShippingFrom,
        PopularityRank = Vm.PopularityRank,
        SizeIds = SelectedSizeIds,
        Colors = Vm.Colors
            .Where(c => !string.IsNullOrWhiteSpace(c.Name))
            .Select(c => new ProductColorInput { Name = c.Name, Hex = c.Hex, ImageUrl = c.ImageUrl })
            .ToList(),
        ColorSizeStocks = ParseStockKeys(InStockColorSizeKeys),
        ImageFileNamesToAdd = imageFileNames,
        ImageAltText = Vm.Name
    };

    private static List<ProductColorSizeStockInput> ParseStockKeys(IEnumerable<string>? keys)
    {
        var result = new List<ProductColorSizeStockInput>();
        foreach (var key in keys ?? Array.Empty<string>())
        {
            var parts = key.Split("||", 2, StringSplitOptions.TrimEntries);
            if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]))
                continue;
            if (!Guid.TryParse(parts[1], out var sizeId))
                continue;
            result.Add(new ProductColorSizeStockInput
            {
                ColorName = parts[0],
                SizeId = sizeId,
                IsInStock = true
            });
        }
        return result;
    }

    private async Task<IReadOnlyList<string>> SaveImageFilesAsync(IEnumerable<IFormFile>? files, string slug)
    {
        if (files is null)
            return Array.Empty<string>();

        var names = new List<string>();
        foreach (var file in files.Where(f => f is { Length: > 0 }))
        {
            await using var stream = file.OpenReadStream();
            var stored = await _imageStorage.SaveProductImageAsync(
                stream,
                file.FileName,
                slug,
                HttpContext.RequestAborted);

            names.Add(string.IsNullOrWhiteSpace(stored.FileNamePreview)
                ? stored.FileNameOriginal
                : stored.FileNamePreview);
        }

        return names;
    }
}

