using GwentApi.Hubs;
using GwentApi.Repository;
using GwentApi.Repository.Interfaces;
using GwentApi.Services;
using GwentApi.Services.Interfaces;

namespace GwentApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddOpenApi();

            var MyAllowSpecificOrigins = "gwentWasmPermission";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.WithOrigins("http://localhost:5140").AllowAnyMethod().AllowAnyHeader();
                                  });
            });
            builder.Services.AddSignalR();

            builder.Services.AddTransient<ILobbyService, LobbyService>();
            builder.Services.AddTransient<IGameService, GameService>();
            builder.Services.AddTransient<IDeckService, DeckService>();
            builder.Services.AddTransient<ICardService, CardService>();
            builder.Services.AddTransient<IStatusService, StatusService>();
            builder.Services.AddTransient<IGameDataService, GameDataService>();
            builder.Services.AddTransient<ICardServiceValidator, CardServiceValidator>();

            builder.Services.AddSingleton<CardsProvider>();
            builder.Services.AddSingleton<ILobbyRepository, LobbyRepository>();//potencjalnie do zmiany na transient w przyszlosci
            builder.Services.AddSingleton<IGameRepository, GameRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthorization();

            app.UseCors(MyAllowSpecificOrigins);

            app.MapHub<GwentHub>("/gwenthub");

            app.MapControllers();

            app.Run();
        }
    }
}
