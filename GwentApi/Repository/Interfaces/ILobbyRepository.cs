using GwentApi.Classes;

namespace GwentApi.Repository.Interfaces
{
    public interface ILobbyRepository
    {
        public Task<Lobby> AddLobby(Lobby lobby);
        public Task<bool> ExistsByCode(string lobbyCode);
        public Task<Lobby> GetLobbyByCode(string lobbyCode);
        Task<PlayerInfo> SetDeckForPlayer(PlayerIdentity identity, string lobbyCode, PlayerInfo playerInfo);
        Task<bool> PlayersReady(string lobbyCode);
        Task<PlayerInfo> GetPlayerInfo(string lobbyCode, PlayerIdentity identity);
        Task SwapDeck(string lobbyCode, PlayerIdentity identity, List<GwentCard> Cards);
    }
}
