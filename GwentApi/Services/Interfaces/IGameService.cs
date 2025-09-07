using GwentApi.Classes;
using GwentApi.Classes.Dtos;

namespace GwentApi.Services.Interfaces
{
    public interface IGameService
    {
        Task<bool> ReadyForGame(string code, PlayerIdentity identity);
        Task<bool> PlayersReady(string code);
        Task<GameStatusDto> GetStatus(string code, PlayerIdentity identity);
    }
}
