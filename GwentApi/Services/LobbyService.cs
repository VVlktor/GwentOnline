using GwentApi.Classes;
using GwentApi.Repository.Interfaces;
using GwentApi.Services.Interfaces;

namespace GwentApi.Services
{
    public class LobbyService : ILobbyService
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly CardsProvider _cardsService;

        public LobbyService(ILobbyRepository lobbyRepository, CardsProvider cardsService)
        {
            _lobbyRepository = lobbyRepository;
            _cardsService = cardsService;
        }

        public async Task<string> CreateLobby()
        {
            string lobbyCode = "";
            do
            {
                lobbyCode = Guid.NewGuid().ToString("N")[..6].ToUpper();
            } while (await _lobbyRepository.ExistsByCode(lobbyCode));

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
            Random rng = new Random();
            playerInfo.CardsSwapped = 0;
            int n = playerInfo.Cards.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (playerInfo.Cards[n], playerInfo.Cards[k]) = (playerInfo.Cards[k], playerInfo.Cards[n]);
            }
            Lobby lobby = await _lobbyRepository.GetLobbyByCode(lobbyCode);

            if (player == PlayerIdentity.PlayerOne)
                lobby.PlayerOneInfo = playerInfo;
            else
                lobby.PlayerTwoInfo = playerInfo;

            await _lobbyRepository.UpdateLobby(lobby);
        }

        public async Task<bool> PlayersReady(string lobbyCode)
        {
            Lobby lobby = await _lobbyRepository.GetLobbyByCode(lobbyCode);
            return lobby.ArePlayersReady();
        }

        public async Task<PlayerInfo> GetPlayerInfo(string lobbyCode, PlayerIdentity identity)
        {
            Lobby lobby = await _lobbyRepository.GetLobbyByCode(lobbyCode);
            PlayerInfo playerInfo = identity == PlayerIdentity.PlayerOne ? lobby.PlayerOneInfo : lobby.PlayerTwoInfo;
            return playerInfo;
        }

        public async Task<PlayerInfo> SwapCard(string lobbyCode, PlayerIdentity player, int id)
        {
            Lobby lobby = await _lobbyRepository.GetLobbyByCode(lobbyCode);
            PlayerInfo playerInfo = player == PlayerIdentity.PlayerOne ? lobby.PlayerOneInfo : lobby.PlayerTwoInfo;

            if (!playerInfo.Cards.Any(x => x.PrimaryId == id)) return playerInfo;
            if (playerInfo.CardsSwapped >= 2) return playerInfo;

            int index = playerInfo.Cards.FindIndex(x => x.PrimaryId == id);
            if (index < 0) return playerInfo;

            GwentCard holder = playerInfo.Cards[10 + playerInfo.CardsSwapped];
            playerInfo.Cards[10 + playerInfo.CardsSwapped] = _cardsService.GetCardByPrimaryId(id);
            playerInfo.Cards[index] = holder;
            playerInfo.CardsSwapped++;

            await _lobbyRepository.UpdateLobby(lobby);

            return playerInfo;
        }
    }
}
