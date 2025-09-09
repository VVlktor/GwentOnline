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

        public async Task<ReadyDto> ReadyForGame(string code, PlayerIdentity identity)
        {
            bool lobbyExists = await _lobbyRepository.ExistsByCode(code);
            if(!lobbyExists) return new(){ Ready=false };

            bool gameExists = await _gameRepository.ExistsByCode(code);
            Game game;

            if (gameExists)
                game = await _gameRepository.GetGameByCode(code);
            else
                game = new(code);

            Lobby lobby = await _lobbyRepository.GetLobbyByCode(code);
            PlayerInfo playerInfo = identity == PlayerIdentity.PlayerOne ? lobby.PlayerOneInfo : lobby.PlayerTwoInfo;

            foreach(var card in playerInfo.Cards)
            {
                card.PrimaryId = lobby.CurrentCardIndex;
                lobby.CurrentCardIndex++;
            }

            await _lobbyRepository.UpdateLobby(lobby);

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

            return new() { Ready=true };
        }

        public async Task<ReadyDto> PlayersReady(string code)
        {
            Game game = await _gameRepository.GetGameByCode(code);
            return new() { Ready = game.PlayersReady() };
        }

        public async Task<GameStatusDto> GetStatus(string code, PlayerIdentity identity, int lastActionId)//TODO: dostosować GameStatusDto
        {
            Game game = await _gameRepository.GetGameByCode(code);

            PlayerSide playerSide = identity == PlayerIdentity.PlayerOne ? game.PlayerOne : game.PlayerTwo;

            List<GwentAction> actions = game.Actions.Where(x => x.Id > lastActionId).ToList();

            GameStatusDto status = new() {
                CardsInHand= playerSide.CardsInHand,
                CardsOnBoard=game.CardsOnBoard,
                Turn=game.Turn,
                Actions=actions
            };
            return status;
        }

        public async Task<StartStatusDto> StartStatus(string code, PlayerIdentity identity)
        {
            Game game = await _gameRepository.GetGameByCode(code);

            PlayerSide playerSide = identity == PlayerIdentity.PlayerOne ? game.PlayerOne : game.PlayerTwo;
            PlayerSide enemySide = identity == PlayerIdentity.PlayerOne ? game.PlayerTwo : game.PlayerOne;

            StartStatusDto status = new()
            {
                PlayerCards = playerSide.CardsInHand,
                EnemyCardsCount=enemySide.CardsInHand.Count(),
                Turn = game.Turn,
                PlayerLeaderCard = playerSide.LeaderCard,
                EnemyLeaderCard = enemySide.LeaderCard,
            };

            return status;
        }
    }
}
