using WearFitter.App.Domain;
using WearFitter.App.Domain.Brands;
using WearFitter.App.Domain.Shoes;
using WearFitter.App.Repository.EFCore.Shoes;
using WearFitter.EFCore.Common;

namespace WearFitter.App.Repository.EFCore;

public class AppUnitOfWork(ApplicationDbContext dbContext) :
    UnitOfWork<ApplicationDbContext>(dbContext), IAppUnitOfWork
{
    //private IShoeFitsRepository? _shoeFitsRepository;
    private IBrandsRepository? _brandsRepository;

    #region Repositories
    //public IShoeFitsRepository ShoeFitsRepository => _shoeFitsRepository ??= new ShoeFitsRepository(DbContext);

    public IBrandsRepository BrandsRepository => _brandsRepository ??= new BrandsRepository(DbContext);

    #endregion
}
