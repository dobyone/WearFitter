using Microsoft.AspNetCore.Components;
using WearFitter.Contracts.Brands;
using WearFitter.Contracts.Brands.Models;

namespace WearFitter.App.Web.Client.Pages.Brands;

public partial class Brands
{
    private IEnumerable<BrandModel>? BrandsList { get; set; }

    [Inject]
    private IBrandsService BrandsService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        BrandsList = await BrandsService.GetAll();

        await base.OnInitializedAsync();
    }
}
