using WearFitter.App.Domain;
using WearFitter.Contracts.Brands;
using WearFitter.Contracts.Brands.Models;
using WearFitter.Domain.Common.Query;
using WearFitter.Services.Server.Brands.Mappers;

namespace WearFitter.Services.Server.Brands;

public class BrandsService(IAppUnitOfWork appUnitOfWork) : IBrandsService
{
    public async Task<IEnumerable<BrandModel>> GetAll()
    {
        var query = new QueryRequest();

        var response = await appUnitOfWork.BrandsRepository.GetAllByFilter(query);

        var result = response.Data.Select(BrandMapper.Map).ToList();

        return result;
    }
}
