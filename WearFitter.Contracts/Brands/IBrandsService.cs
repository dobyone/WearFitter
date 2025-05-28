using WearFitter.Contracts.Brands.Models;

namespace WearFitter.Contracts.Brands;

public interface IBrandsService
{
    Task<IEnumerable<BrandModel>> GetAll();
}
