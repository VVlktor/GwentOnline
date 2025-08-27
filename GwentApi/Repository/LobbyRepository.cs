using GwentApi.Classes;
using GwentApi.Repository.Interfaces;

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
    }
}
