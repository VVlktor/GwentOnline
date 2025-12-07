using GwentShared.Classes;

namespace GwentApi.Services.Interfaces
{
    public interface ILobbyService
    {
        public Task<string> CreateLobby();
        public Task<bool> JoinLobby(string lobbyCode);
        Task SetDeck(string lobbyCode, PlayerIdentity player, PlayerDeckInfo playerInfo);
        Task<bool> PlayersReady(string lobbyCode);
        Task<PlayerInfo> GetPlayerInfo(string lobbyCode, PlayerIdentity identity);
        Task<PlayerInfo> SwapCard(string lobbyCode, PlayerIdentity player, int id);
    }
}
