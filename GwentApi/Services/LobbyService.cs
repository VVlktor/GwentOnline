using GwentApi.Classes;
using GwentApi.Repository.Interfaces;
using GwentApi.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

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

        public async Task SetDeck(string lobbyCode, int player, PlayerInfo playerInfo)
        {
            PlayerIdentity identity = player == 1 ? PlayerIdentity.PlayerOne : PlayerIdentity.PlayerTwo;
            await _lobbyRepository.SetDeckForPlayer(identity, lobbyCode, playerInfo);
        }
    }
}
