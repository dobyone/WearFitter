using WearFitter.App.Domain.Shoes;
using WearFitter.App.Domain.Shoes.Models;

namespace WearFitter.App.Repository.EFCore.Shoes;

public class ShoeFitsRepository(ApplicationDbContext dbContext) :
    AppBaseRepository<ShoeFit>(dbContext),
    IShoeFitsRepository
{
}
