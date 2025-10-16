using GwentApi.Classes;

namespace GwentApi.Repository.Interfaces
{
    public interface IGameRepository
    {
        Task<Game> GetGameByCode(string code);
        Task<Game> UpdateGame(Game game);
        Task<Game> AddGame(Game game);
        Task<bool> ExistsByCode(string code);
        Task<List<Game>> GetAllGames();//tylko do testowania
        Task RemoveGame(Game game);
    }
}
