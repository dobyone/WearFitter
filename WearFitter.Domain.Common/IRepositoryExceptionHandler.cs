namespace WearFitter.Domain.Common;

public interface IRepositoryExceptionHandler
{
    bool TryHandle(Exception exception);
}

public interface IRepositoryException
{
}

public interface IRepositoryExceptionHandler<T> : IRepositoryExceptionHandler
    where T : IRepositoryException
{
}
