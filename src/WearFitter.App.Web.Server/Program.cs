using Microsoft.EntityFrameworkCore;
using WearFitter.App.Domain;
using WearFitter.App.Repository.EFCore;
using WearFitter.EFCore.Common.Services;

namespace WearFitter.App.Web.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<ApplicationDbContext>(o =>
                    {
                        o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
                        //o.EnableSensitiveDataLogging(Environment.IsDevelopment());
                        //o.EnableDetailedErrors(Environment.IsDevelopment());
                    }, contextLifetime: ServiceLifetime.Transient
                )
                .AddTransient<IAppUnitOfWork, AppUnitOfWork>()
                .AddHostedService<DatabaseMigrationsBackgroundService<ApplicationDbContext>>();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

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
