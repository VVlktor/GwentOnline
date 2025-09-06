using GwentApi.Classes;

namespace GwentApi.Repository.Interfaces
{
    public interface ILobbyRepository
    {
        Task<Lobby> AddLobby(Lobby lobby);
        Task<bool> ExistsByCode(string lobbyCode);
        Task<Lobby> GetLobbyByCode(string lobbyCode);
        Task<Lobby> UpdateLobby(Lobby lobby);
    }
}
