using WearFitter.App.Domain.Brands;
using WearFitter.App.Domain.Brands.Models;

namespace WearFitter.App.Repository.EFCore.Shoes;

public class BrandsRepository(ApplicationDbContext dbContext) :
    AppBaseRepository<Brand>(dbContext),
    IBrandsRepository
{
}
