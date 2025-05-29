using Microsoft.EntityFrameworkCore;
using WearFitter.App.Domain.Brands.Models;

namespace WearFitter.App.Repository.EFCore.Brands.Builders;

internal class BrandsBuilder
{
    internal static void Build(ModelBuilder builder)
    {
        builder.Entity<Brand>(mission =>
        {
            mission.HasKey(m => m.Id);

            mission.Property(m => m.Name)
                .IsRequired();

            mission.HasIndex(m => new { m.Name })
                .IsUnique();
        });
    }
}
