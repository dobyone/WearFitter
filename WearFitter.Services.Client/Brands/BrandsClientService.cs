using WearFitter.Contracts.Brands;
using WearFitter.Contracts.Brands.Models;
using WearFitter.Services.Server;
using WearFitter.Web.Infrastructure.Extensions;

namespace WearFitter.Services.Client.Brands;

public class BrandsClientService(IHttpClientFactory httpClientFactory) : IBrandsService
{
    private const string ENDPOINT = "brands";

    private HttpClient Client { get; } = httpClientFactory.CreateClient(ApiConstants.ApiClientName);

    public async Task<IEnumerable<BrandModel>> GetAll()
    {
        var brands = await Client.GetFromNJsonAsync<IEnumerable<BrandModel>>(ENDPOINT);

        return brands;
    }
}
