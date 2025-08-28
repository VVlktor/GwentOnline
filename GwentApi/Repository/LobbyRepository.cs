using GwentApi.Classes;
using GwentApi.Repository.Interfaces;
using System.Security;

namespace GwentApi.Repository
{
    public class LobbyRepository : ILobbyRepository
    {
        private List<Lobby> lobbyList = new();//do zmiany na baze danych

        public async Task<Lobby> AddLobby(Lobby lobby)
        {
            lobbyList.Add(lobby);
            return lobby;
        }

        public async Task<bool> ExistsByCode(string lobbyCode)
        {
            return lobbyList.Any(x => x.LobbyCode == lobbyCode);
        }

        public async Task<Lobby> GetLobbyByCode(string lobbyCode)
        {
            return lobbyList.FirstOrDefault(x => x.LobbyCode == lobbyCode);
        }

        public async Task<PlayerInfo> SetDeckForPlayer(PlayerIdentity identity, string lobbyCode, PlayerInfo playerInfo)
        {
            Lobby lobby = lobbyList.FirstOrDefault(x=>x.LobbyCode==lobbyCode);
            if(identity == PlayerIdentity.PlayerOne)
                lobby.PlayerOneInfo = playerInfo;
            else
                lobby.PlayerTwoInfo = playerInfo;
            return playerInfo;
        }

        public async Task<bool> PlayersReady(string lobbyCode)
        {
            Lobby lobby = lobbyList.FirstOrDefault(x => x.LobbyCode == lobbyCode);
            return lobby.ArePlayersReady();
        }
    }
}
