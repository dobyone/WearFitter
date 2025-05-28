using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WearFitter.Contracts.Brands;
using WearFitter.Services.Client.Brands;

namespace WearFitter.App.Web.Client;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.Services.AddTransient<IBrandsService, BrandsClientService>();

        await builder.Build().RunAsync();
    }
}
