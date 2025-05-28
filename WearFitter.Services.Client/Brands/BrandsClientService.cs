using System.Net;
using WearFitter.Contracts.Brands;
using WearFitter.Contracts.Brands.Models;
using WearFitter.Web.Infrastructure.Extensions;

namespace WearFitter.Services.Client.Brands;

public class BrandsClientService(HttpClient client) : IBrandsService
{
    private const string ENDPOINT = "api/brands";

    public async Task<IEnumerable<BrandModel>> GetAll()
    {
        var brands = await client.GetFromNJsonAsync<IEnumerable<BrandModel>>(ENDPOINT);

        return brands;
    }
}
