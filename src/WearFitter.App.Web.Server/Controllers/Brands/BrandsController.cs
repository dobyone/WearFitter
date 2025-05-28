using Microsoft.AspNetCore.Mvc;
using WearFitter.Contracts.Brands;
using WearFitter.Contracts.Brands.Models;

namespace WearFitter.App.Web.Server.Controllers.Brands;

[ApiController]
[Route("api/[controller]")]
public class BrandsController(IBrandsService brandsService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BrandModel>))]
    public async Task<IActionResult> GetAll()
    {
        var result = await brandsService.GetAll();

        return Ok(result.ToList());
    }
}
