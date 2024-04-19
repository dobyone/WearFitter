using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WearFitter.App.Repository.EFCore;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    ILoggerFactory loggerFactory) :
    DbContext(options),
    IDataProtectionKeyContext
{
    private readonly ILoggerFactory loggerFactory = loggerFactory;

    public DbSet<DataProtectionKey> DataProtectionKeys => throw new NotImplementedException();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseLoggerFactory(loggerFactory);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}