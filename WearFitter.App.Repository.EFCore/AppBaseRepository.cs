using WearFitter.Domain.Common;
using WearFitter.EFCore.Common;

namespace WearFitter.App.Repository.EFCore;

public abstract class AppBaseRepository<T> : BaseRepository<ApplicationDbContext, T>, IRepository<T> where T : class
{
    protected AppBaseRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }
}
