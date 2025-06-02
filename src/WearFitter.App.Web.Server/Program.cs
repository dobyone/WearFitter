using Microsoft.EntityFrameworkCore;
using WearFitter.App.Domain;
using WearFitter.App.Repository.EFCore;
using WearFitter.Contracts.Brands;
using WearFitter.EFCore.Common.Services;
using WearFitter.Services.Server.Brands;

namespace WearFitter.App.Web.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddJsonFile($"appsettings.json");
        builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json");

        Console.WriteLine($"----configuration: {builder.Configuration.GetConnectionString("DefaultConnection")}");
        builder.Services.AddDbContext<ApplicationDbContext>(o =>
                    {
                        o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
                        //o.EnableSensitiveDataLogging(Environment.IsDevelopment());
                        //o.EnableDetailedErrors(Environment.IsDevelopment());
                    }, contextLifetime: ServiceLifetime.Transient
                )
                .AddTransient<IAppUnitOfWork, AppUnitOfWork>()
                .AddHostedService<DatabaseMigrationsBackgroundService<ApplicationDbContext>>()
                .AddTransient<IBrandsService, BrandsService>();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        builder.Services.AddControllers();
        //builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        if (!app.Environment.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<Components.App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

        app.Run();
    }
}
