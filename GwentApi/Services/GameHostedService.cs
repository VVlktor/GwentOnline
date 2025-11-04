using GwentApi.Repository.Interfaces;

namespace GwentApi.Services
{
    public class GameHostedService : BackgroundService
    {
        private IGameRepository _gameRepository;
        private ILobbyRepository _lobbyRepository;

        public GameHostedService(IGameRepository gameRepository, ILobbyRepository lobbyRepository)
        {
            _gameRepository = gameRepository;
            _lobbyRepository = lobbyRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var games = await _gameRepository.GetAllGames();
                var oldGames = games.Where(x => x.LastUpdate.AddMinutes(30) < DateTime.Now).ToList();
                await _gameRepository.RemoveRange(oldGames);

                var lobby = await _lobbyRepository.GetAll();
                var oldLobby = lobby.Where(x => x.CreatedAt.AddMinutes(30) < DateTime.Now).ToList();
                await _lobbyRepository.RemoveRange(oldLobby);

                await Task.Delay(600000, stoppingToken);
            }
        }
    }
}
