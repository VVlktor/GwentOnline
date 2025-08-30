using GwentWebAssembly.Data;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface IDeckService
    {
        Task<ResponseData> VerifyDeck(PlayerInfo playerInfo);
        Task SetDeck(PlayerInfo playerInfo);
        Task<bool> PlayersReady(string lobbyCode);
        Task<PlayerInfo> GetPlayerInfo();
        Task SwapDeck(List<GwentCard> cards);
    }
}
