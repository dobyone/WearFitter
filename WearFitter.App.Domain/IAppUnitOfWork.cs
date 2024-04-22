using WearFitter.App.Domain.Brands;
using WearFitter.Domain.Common;

namespace WearFitter.App.Domain;

public interface IAppUnitOfWork : IUnitOfWork
{
    //IShoeFitsRepository ShoeFitsRepository { get; }

    IBrandsRepository BrandsRepository { get; }
}