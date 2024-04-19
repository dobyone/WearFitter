namespace WearFitter.Domain.Common;

public interface IUnitOfWork
{
    Task<int> Complete();

    Task<bool> TryComplete(params IRepositoryExceptionHandler[] handlers);
}
