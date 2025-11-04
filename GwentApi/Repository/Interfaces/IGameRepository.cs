using GwentApi.Classes;

namespace GwentApi.Repository.Interfaces
{
    public interface IGameRepository
    {
        Task<bool> ExistsByCode(string code);
        Task<Game> GetGameByCode(string code);

        Task<List<Game>> GetAllGames();//tylko do testowania

        Task<Game> UpdateGame(Game game);
        
        Task<Game> AddGame(Game game);
        Task<Game> AddRange(IEnumerable<Game> games);

        Task RemoveGame(Game game);
        Task RemoveRange(List<Game> games);
    }
}
