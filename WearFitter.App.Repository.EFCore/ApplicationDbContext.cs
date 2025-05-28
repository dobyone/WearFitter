using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WearFitter.App.Domain.Brands.Models;
using WearFitter.App.Repository.EFCore.Brands.Builders;

namespace WearFitter.App.Repository.EFCore;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    ILoggerFactory loggerFactory) :
    DbContext(options)
{
    private readonly ILoggerFactory loggerFactory = loggerFactory;

    public DbSet<Brand> Brands { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseLoggerFactory(loggerFactory);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        BrandsBuilder.Build(builder);
    }
}