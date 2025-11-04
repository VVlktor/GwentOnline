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

        public async Task AddRange(List<Lobby> lobbiesList)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsByCode(string lobbyCode)
        {
            return lobbyList.Any(x => x.LobbyCode == lobbyCode);
        }

        public async Task<IEnumerable<Lobby>> GetAll()
        {
            return lobbyList;
        }

        public async Task<Lobby> GetLobbyByCode(string lobbyCode)
        {
            return lobbyList.FirstOrDefault(x => x.LobbyCode == lobbyCode);
        }

        public async Task RemoveRange(List<Lobby> lobbiesList)
        {
            foreach(var lobby in lobbiesList)
                lobbyList.Remove(lobby);
        }

        public async Task<Lobby> UpdateLobby(Lobby lobby)
        {
            int index = lobbyList.FindIndex(x => x.LobbyCode == lobby.LobbyCode);
            if (index >= 0)
                lobbyList[index] = lobby;
            return lobby;
        }
    }
}
