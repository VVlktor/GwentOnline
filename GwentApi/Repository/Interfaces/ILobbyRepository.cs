using GwentApi.Classes;

namespace GwentApi.Repository.Interfaces
{
    public interface ILobbyRepository
    {
        public Lobby AddLobby(Lobby lobby);
        public bool ExistsByCode(string lobbyCode);
        public Lobby GetLobbyByCode(string lobbyCode);
    }
}
