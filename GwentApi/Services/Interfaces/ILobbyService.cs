using GwentApi.Classes;

namespace GwentApi.Services.Interfaces
{
    public interface ILobbyService
    {
        public Task<string> CreateLobby();
        public Task<bool> JoinLobby(string lobbyCode);
        Task SetDeck(string lobbyCode, PlayerIdentity player, PlayerInfo playerInfo);
        Task<bool> PlayersReady(string lobbyCode);
        Task<PlayerInfo> GetPlayerInfo(string lobbyCode, PlayerIdentity identity);
        Task SwapCards(string lobbyCode, PlayerIdentity player, List<GwentCard> Cards);
    }
}
