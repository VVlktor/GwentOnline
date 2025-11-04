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
        private IGameDataService _gameDataService;

        public GameService(IGameRepository gameRepository, ILobbyRepository lobbyRepository, IGameDataService gameDataService)
        {
            _gameRepository = gameRepository;
            _lobbyRepository = lobbyRepository;
            _gameDataService = gameDataService;
        }

        public async Task<ReadyDto> ReadyForGame(string code, PlayerIdentity identity)
        {
            Random rnd = new();

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

            GwentLeaderCard leaderCard = new()
            {
                PrimaryId = playerInfo.LeaderCard.PrimaryId,
                CardId = playerInfo.LeaderCard.CardId,
                Abilities = playerInfo.LeaderCard.Abilities,
                Faction = playerInfo.LeaderCard.Faction,
                FileName = playerInfo.LeaderCard.FileName,
                LeaderActive = false,
                LeaderAvailable = true,
                Strength = playerInfo.LeaderCard.Strength,
                Name = playerInfo.LeaderCard.Name,
                Placement = playerInfo.LeaderCard.Placement,
            };

            PlayerSide playerSide = new()
            {
                LeaderCard = leaderCard,
                CardsInHand = playerInfo.Cards[..10],
                Deck = playerInfo.Cards[10..],
                Faction = playerInfo.Faction,
                UsedCards = new(),
                Hp=2
            };

            if (playerSide.LeaderCard.CardId == 142)
            {
                GwentCard card = playerSide.Deck[rnd.Next(playerSide.Deck.Count)];
                playerSide.Deck.Remove(card);
                playerSide.CardsInHand.Add(card);
                playerSide.LeaderCard.LeaderAvailable = false;
            }

            _gameDataService.SetPlayerSide(game, playerSide, identity);
            _gameDataService.SetReady(game, identity);

            game.Turn = rnd.Next(1, 3) == 1 ? PlayerIdentity.PlayerTwo : PlayerIdentity.PlayerOne;

            if (gameExists)
            {
                if (game.PlayerOne.Faction == CardFaction.NilfgaardianEmpire && game.PlayerTwo.Faction != CardFaction.NilfgaardianEmpire)//chwila, serio cos takiego bylo? do sprawdzenia
                    game.Turn = PlayerIdentity.PlayerOne;
                else if (game.PlayerOne.Faction != CardFaction.NilfgaardianEmpire && game.PlayerTwo.Faction == CardFaction.NilfgaardianEmpire)
                    game.Turn = PlayerIdentity.PlayerTwo;

                if(game.PlayerOne.LeaderCard.CardId == 59 || game.PlayerTwo.LeaderCard.CardId == 59)
                {
                    game.PlayerOne.LeaderCard.LeaderAvailable = false;
                    game.PlayerTwo.LeaderCard.LeaderAvailable = false;
                }
            }

            if(gameExists)
                await _gameRepository.UpdateGame(game);
            else
                await _gameRepository.AddGame(game);//tworze gre ale nigdy nie usuwam - dodac jakiegos background workera

            return new() { Ready=true };
        }

        public async Task<ReadyDto> PlayersReady(string code)
        {
            Game game = await _gameRepository.GetGameByCode(code);
            return new() { Ready = _gameDataService.PlayersReady(game) };
        }

        public async Task<GameStatusDto> GetStatus(string code, PlayerIdentity identity)
        {
            Game game = await _gameRepository.GetGameByCode(code);

            PlayerSide playerSide = _gameDataService.GetPlayerSide(game, identity);
            PlayerSide enemySide = _gameDataService.GetEnemySide(game, identity);

            GwentAction action = game.Actions.Last();

            GameStatusDto status = new() {
                CardsInHand= playerSide.CardsInHand,
                CardsOnBoard=game.CardsOnBoard,
                UsedCards=playerSide.UsedCards,
                Turn=game.Turn,
                Action=action,
                EnemyCardsCount=enemySide.CardsInHand.Count,
                EnemyUsedCardsCount=enemySide.UsedCards.Count,
                PlayerDeckCount=playerSide.Deck.Count,
                EnemyDeckCount=enemySide.Deck.Count,
                EnemyHp=enemySide.Hp,
                PlayerHp=playerSide.Hp,
                PlayerLeaderAvailable=playerSide.LeaderCard.LeaderAvailable,
                EnemyLeaderAvailable=enemySide.LeaderCard.LeaderAvailable,
                PlayerPassed = identity == PlayerIdentity.PlayerOne ? game.HasPassed.PlayerOne : game.HasPassed.PlayerTwo,
                EnemyPassed = identity == PlayerIdentity.PlayerOne ? game.HasPassed.PlayerTwo : game.HasPassed.PlayerOne,
            };

            return status;
        }

        public async Task<StartStatusDto> StartStatus(string code, PlayerIdentity identity)
        {
            Game game = await _gameRepository.GetGameByCode(code);
            
            PlayerSide playerSide = _gameDataService.GetPlayerSide(game, identity);
            PlayerSide enemySide = _gameDataService.GetEnemySide(game, identity);

            StartStatusDto status = new()
            {
                PlayerCards = playerSide.CardsInHand,
                EnemyDeckCount=enemySide.Deck.Count,
                Turn = game.Turn,
                PlayerLeaderCard = playerSide.LeaderCard,
                EnemyLeaderCard = enemySide.LeaderCard,
                PlayerDeckCount = playerSide.Deck.Count,
                EnemyCardsInHandCount=enemySide.CardsInHand.Count
            };

            return status;
        }
    }
}
