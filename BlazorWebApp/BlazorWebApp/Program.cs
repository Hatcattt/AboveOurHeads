using AboveOurHeads.Core.Interfaces;
using AboveOurHeads.Services;
using AboveOurHeads.Services.Tles;
using BlazorWebApp.Components;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();

            builder.Services.AddMemoryCache();

            builder.Services.AddHttpClient<HttpTleProvider>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(15);
            });

            builder.Services.AddSingleton<ITleProvider>(sp =>
                new CachedTleProvider(
                    sp.GetRequiredService<ILogger<CachedTleProvider>>(),
                    sp.GetRequiredService<HttpTleProvider>(),
                    sp.GetRequiredService<IMemoryCache>(),
                    TimeSpan.FromMinutes(10)));

            builder.Services.AddScoped<ISatelliteService, SatelliteService>();

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

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            app.Run();
        }
    }
}
