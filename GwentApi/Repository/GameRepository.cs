using GwentApi.Classes;
using GwentApi.Repository.Interfaces;

namespace GwentApi.Repository
{
    public class GameRepository : IGameRepository
    {
        private List<Game> _games = new();//bedzie zmienione na baze danych

        public async Task<Game> GetGameByCode(string code)
        {
            return _games.First(x => x.Code == code);
        }

        public async Task<Game> AddGame(Game game)
        {
            _games.Add(game);
            return game;
        }

        public async Task<Game> UpdateGame(Game game)
        {
            int index = _games.FindIndex(x => x.Code == game.Code);
            if (index >= 0)
                _games[index] = game;
            return game;
        }

        public async Task<bool> ExistsByCode(string code)
        {
            return _games.Any(x => x.Code == code);
        }
    }
}
