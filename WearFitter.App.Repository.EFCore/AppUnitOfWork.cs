using WearFitter.App.Domain;
using WearFitter.EFCore.Common;

namespace WearFitter.App.Repository.EFCore;

public class AppUnitOfWork(ApplicationDbContext dbContext) :
    UnitOfWork<ApplicationDbContext>(dbContext), IAppUnitOfWork
{

    #region Repositories

    #endregion
}
