using GwentApi.Classes;
using GwentApi.Repository.Interfaces;

namespace GwentApi.Repository
{
    public class LobbyRepository : ILobbyRepository
    {
        private List<Lobby> lobbyList = new();//do zmiany na baze danych

        public Lobby AddLobby(Lobby lobby)
        {
            lobbyList.Add(lobby);
            return lobby;
        }

        public bool ExistsByCode(string lobbyCode)
        {
            return lobbyList.Any(x => x.LobbyCode == lobbyCode);
        }

        public Lobby GetLobbyByCode(string lobbyCode)
        {
            return lobbyList.FirstOrDefault(x => x.LobbyCode == lobbyCode);
        }
    }
}
