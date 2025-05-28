using WearFitter.App.Domain.Brands.Models;
using WearFitter.Contracts.Brands.Models;

namespace WearFitter.Services.Server.Brands.Mappers;

internal class BrandMapper
{
    public static BrandModel Map(Brand source)
    {
        BrandModel targetModel = new()
        {
            Id = source.Id,
            Name = source.Name,
        };

        return targetModel;
    }
}
