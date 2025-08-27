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

        public async Task<string> CreateLobby()
        {
            string lobbyCode = "";
            do
            {
                lobbyCode = Guid.NewGuid().ToString("N")[..6].ToUpper();
            }while(await _lobbyRepository.ExistsByCode(lobbyCode));

            Lobby newLobby = new(lobbyCode);
            await _lobbyRepository.AddLobby(newLobby);
            return lobbyCode;
        }

        public async Task<bool> JoinLobby(string lobbyCode)
        {
            if (!await _lobbyRepository.ExistsByCode(lobbyCode))
                return false;

            Lobby lobby = await _lobbyRepository.GetLobbyByCode(lobbyCode);
            if (lobby.PlayersCount >= 2)
                return false;

            lobby.PlayersCount++;
            return true;
        }
    }
}
