
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

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var MyAllowSpecificOrigins = "somePermissions";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.WithOrigins("http://localhost:5140").AllowAnyMethod().AllowAnyHeader();
                                  });
            });

            builder.Services.AddTransient<ILobbyService, LobbyService>();
            builder.Services.AddTransient<IGameService, GameService>();
            builder.Services.AddTransient<IDeckService, DeckService>();


            builder.Services.AddSingleton<ILobbyRepository, LobbyRepository>();//potencjalnie do zmiany na transient w przyszlosci

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthorization();

            app.UseCors(MyAllowSpecificOrigins);

            app.MapControllers();

            app.Run();
        }
    }
}
