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

        public async Task SetDeck(string lobbyCode, PlayerIdentity player, PlayerInfo playerInfo)
        {
            await _lobbyRepository.SetDeckForPlayer(player, lobbyCode, playerInfo);
        }

        public async Task<bool> PlayersReady(string lobbyCode)
        {
            return await _lobbyRepository.PlayersReady(lobbyCode);
        }

        public async Task<PlayerInfo> GetPlayerInfo(string lobbyCode, PlayerIdentity identity)
        {
            PlayerInfo playerInfo = await _lobbyRepository.GetPlayerInfo(lobbyCode, identity);
            return playerInfo;
        }

        public async Task SwapCards(string lobbyCode, PlayerIdentity player, List<GwentCard> Cards)
        {
            await _lobbyRepository.SwapDeck(lobbyCode, player, Cards);
        }
    }
}
