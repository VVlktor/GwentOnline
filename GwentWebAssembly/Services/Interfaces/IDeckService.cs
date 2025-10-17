using GwentWebAssembly.Data;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface IDeckService
    {
        Task<ResponseData> VerifyAndSetDeck(PlayerDeckInfo playerInfo);
        Task<bool> PlayersReady(string lobbyCode);
        Task<PlayerInfo> GetPlayerInfo();
        Task<PlayerInfo> SwapCardInDeck(int id);
        Task<bool> SetReady();
        Task<bool> GameReady();
    }
}
