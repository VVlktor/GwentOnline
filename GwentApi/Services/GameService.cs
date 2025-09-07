using GwentApi.Classes;
using GwentApi.Classes.Dtos;
using GwentApi.Repository.Interfaces;
using GwentApi.Services.Interfaces;

namespace GwentApi.Services
{
    public class GameService : IGameService
    {
        private IGameRepository _gameRepository;
        private ILobbyRepository _lobbyRepository;

        public GameService(IGameRepository gameRepository, ILobbyRepository lobbyRepository)
        {
            _gameRepository = gameRepository;
            _lobbyRepository = lobbyRepository;
        }

        public async Task<bool> ReadyForGame(string code, PlayerIdentity identity)
        {
            bool lobbyExists = await _lobbyRepository.ExistsByCode(code);
            if(!lobbyExists) return false;

            bool gameExists = await _gameRepository.ExistsByCode(code);
            Game game;

            if (gameExists)
                game = await _gameRepository.GetGameByCode(code);
            else
                game = new(code);

            Lobby lobby = await _lobbyRepository.GetLobbyByCode(code);
            PlayerInfo playerInfo = identity == PlayerIdentity.PlayerOne ? lobby.PlayerOneInfo : lobby.PlayerTwoInfo;
            PlayerSide playerSide = new()
            {
                LeaderCard = playerInfo.LeaderCard,
                CardsInHand = playerInfo.Cards[..10],
                Deck = playerInfo.Cards[10..],
                Faction = playerInfo.Faction,
                UsedCards = new()
            };

            game.SetPlayerSide(playerSide, identity);
            game.SetReady(identity);

            await _gameRepository.AddGame(game);

            return true;
        }

        public async Task<bool> PlayersReady(string code)
        {
            Game game = await _gameRepository.GetGameByCode(code);
            return game.PlayersReady();
        }

        public async Task<GameStatusDto> GetStatus(string code, PlayerIdentity identity)
        {
            GameStatusDto status = new() {
                
            };
            return status;
        }
    }
}
