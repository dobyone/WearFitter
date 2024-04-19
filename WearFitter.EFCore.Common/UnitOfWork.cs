using Microsoft.EntityFrameworkCore;
using WearFitter.Domain.Common;

namespace WearFitter.EFCore.Common;

public abstract class UnitOfWork<TContext>(TContext dbContext) :
    IUnitOfWork
    where TContext : DbContext
{
    protected TContext DbContext { get; } = dbContext;

    public Task<int> Complete() => DbContext.SaveChangesAsync();

    public async Task<bool> TryComplete(params IRepositoryExceptionHandler[] handlers)
    {
        try
        {
            await Complete();
        }
        catch (Exception ex)
        {
            bool handled = false;

            foreach (IRepositoryExceptionHandler handler in handlers)
            {
                handled = handler.TryHandle(ex);

                if (handled) break;
            }

            if (!handled) throw;

            return false;
        }

        return true;
    }
}
