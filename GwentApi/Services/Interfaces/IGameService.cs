using GwentApi.Classes;
using GwentApi.Classes.Dtos;

namespace GwentApi.Services.Interfaces
{
    public interface IGameService
    {
        Task<ReadyDto> ReadyForGame(string code, PlayerIdentity identity);
        Task<ReadyDto> PlayersReady(string code);
        Task<GameStatusDto> GetStatus(string code, PlayerIdentity identity);
        Task<StartStatusDto> StartStatus(string code, PlayerIdentity identity);
    }
}
