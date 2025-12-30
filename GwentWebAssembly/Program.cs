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

            builder.Services.AddLocalStorageServices();

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddTransient<ICardService, CardService>();
            builder.Services.AddTransient<ILobbyInitializationService, LobbyInitializationService>();
            builder.Services.AddTransient<IDeckService, DeckService>();
            builder.Services.AddTransient<IGameService, GameService>();
            builder.Services.AddTransient<IAnimationService, AnimationService>();
            builder.Services.AddTransient<ILobbySetupService, LobbySetupService>();
            builder.Services.AddTransient<IClipboardService, ClipboardService>();

            builder.Services.AddSingleton<IDataService, DataService>();
            builder.Services.AddSingleton<IStatusService, StatusService>();
            builder.Services.AddSingleton<IPlayerService, PlayerService>();
            builder.Services.AddSingleton<IGwentHubService, GwentHubService>();
            builder.Services.AddSingleton<ICarouselService, CarouselService>();

            await builder.Build().RunAsync();
        }
    }
}
