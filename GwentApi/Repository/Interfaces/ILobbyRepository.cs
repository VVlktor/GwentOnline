using GwentApi.Classes;

namespace GwentApi.Repository.Interfaces
{
    public interface ILobbyRepository
    {
        public Task<Lobby> AddLobby(Lobby lobby);
        public Task<bool> ExistsByCode(string lobbyCode);
        public Task<Lobby> GetLobbyByCode(string lobbyCode);
    }
}
