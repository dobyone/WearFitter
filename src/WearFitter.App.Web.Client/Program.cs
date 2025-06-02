using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WearFitter.Contracts.Brands;
using WearFitter.Services.Client.Brands;
using Microsoft.Extensions.Http;

namespace WearFitter.App.Web.Client;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.Services.AddTransient<IBrandsService, BrandsClientService>();

        builder.Services.AddHttpClient(
                "API",
                client =>
                {
                    client.BaseAddress = new Uri("api");
                    client.Timeout = TimeSpan.FromSeconds(30);
                });

        await builder.Build().RunAsync();
    }
}
