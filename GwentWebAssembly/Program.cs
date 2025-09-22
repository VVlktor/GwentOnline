using GwentWebAssembly.Services;
using GwentWebAssembly.Services.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace GwentWebAssembly
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddTransient<CardService>();
            builder.Services.AddTransient<IHomePageService, HomePageService>();
            builder.Services.AddTransient<IDeckService, DeckService>();
            builder.Services.AddTransient<IGameService, GameService>();
            builder.Services.AddTransient<IGwentHubService, GwentHubService>();
            builder.Services.AddTransient<IAnimationService, AnimationService>();

            builder.Services.AddScoped<IStatusService, StatusService>();

            builder.Services.AddSingleton<PlayerService>();

            await builder.Build().RunAsync();
        }
    }
}
