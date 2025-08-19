using GwentApi.Classes;
using GwentApi.Repository.Interfaces;
using GwentApi.Services.Interfaces;

namespace GwentApi.Services
{
    public class LobbyService : ILobbyService
    {
        private readonly ILobbyRepository _lobbyRepository;

        public LobbyService(ILobbyRepository lobbyRepository)
        {
            _lobbyRepository = lobbyRepository;
        }

        public string CreateLobby()
        {
            string lobbyCode = Guid.NewGuid().ToString("N")[..6].ToUpper();

            while (_lobbyRepository.ExistsByCode(lobbyCode))
                lobbyCode = Guid.NewGuid().ToString("N")[..6].ToUpper();

            Lobby newLobby = new(lobbyCode);
            _lobbyRepository.AddLobby(newLobby);
            return lobbyCode;
        }

        public bool JoinLobby(string lobbyCode)
        {
            if (!_lobbyRepository.ExistsByCode(lobbyCode))
                return false;

            Lobby lobby = _lobbyRepository.GetLobbyByCode(lobbyCode);
            if (lobby.PlayersCount >= 2)
                return false;

            lobby.PlayersCount++;
            return true;
        }
    }
}
