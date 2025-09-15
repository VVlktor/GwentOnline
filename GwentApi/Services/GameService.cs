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

        public async Task<GameStatusDto> GetStatus(string code, PlayerIdentity identity)//TODO: WAZNE VERY IMPORTANT ACTIONS SIE SPIERDOLIŁY (znaczy ja je spierdoliłem) - NAPRAWIC
        {
            Game game = await _gameRepository.GetGameByCode(code);

            PlayerSide playerSide = identity == PlayerIdentity.PlayerOne ? game.PlayerOne : game.PlayerTwo;

            GwentAction action = game.Actions.Last();//potencjalnie bedzie sie wywalac jesli nie ma zadnej akcji - pytanie czy moze nie byc zadnej akcji na tym etapie. do sprawdzenia

            GameStatusDto status = new() {
                CardsInHand= playerSide.CardsInHand,
                CardsOnBoard=game.CardsOnBoard,
                Turn=game.Turn,
                Action=action
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
                EnemyDeckCount=enemySide.CardsInHand.Count(),
                Turn = game.Turn,
                PlayerLeaderCard = playerSide.LeaderCard,
                EnemyLeaderCard = enemySide.LeaderCard,
                PlayerDeckCount = playerSide.Deck.Count()
            };

            return status;
        }

        public async Task<bool> LaneClicked(LaneClickedDto laneClickedDto)
        {
            Game game = await _gameRepository.GetGameByCode(laneClickedDto.Code);
            PlayerSide playerSide = game.GetPlayerSide(laneClickedDto.Identity);

            if (game.Turn != laneClickedDto.Identity) return false;

            if (playerSide.CardsInHand.Any(x => x.PrimaryId == laneClickedDto.Card.PrimaryId)) return false;
            
            bool isPlacementAcceptable = laneClickedDto.Card.Placement switch
            {
                TroopPlacement.Melee => (GwentLane.Melee == laneClickedDto.Lane),
                TroopPlacement.Range => (GwentLane.Range == laneClickedDto.Lane),
                TroopPlacement.Siege => (GwentLane.Siege == laneClickedDto.Lane),
                TroopPlacement.Agile => (GwentLane.Melee == laneClickedDto.Lane || GwentLane.Range == laneClickedDto.Lane),
                _ => false
            };//dodac opcje szpiegów (GwentLine.EnemyMelee itp)

            if(!isPlacementAcceptable) return false;

            //koniec sprawdzania
            GwentCard card = playerSide.CardsInHand.First(x => x.PrimaryId == laneClickedDto.Card.PrimaryId);
            GwentBoardCard boardCard = new()
            {
                Name = card.Name,
                PrimaryId = card.PrimaryId,
                CardId = card.CardId,
                Faction = card.Faction,
                Description = card.Description,
                Placement = card.Placement,//ale usunąć agile - zmienic na melee lub range
                Strength = card.Strength,
                Abilities = card.Abilities,
                CurrentStrength = card.Strength,//zalezy od podejscia, ale bedzie trzeba to zmienic tak czy inaczej - nie wiem jeszcze kiedy bede liczyl sily jednostek (pewnie tuz przed zwroceniem w hubie)
                Owner = laneClickedDto.Identity
            };
            playerSide.CardsInHand.Remove(card);
            game.CardsOnBoard.Add(boardCard);//robione przy mega spanku, sprawdzic czy jest git

            List<GwentBoardCard> affectedCards = new();
           //tutaj potrzebuje listy affected card, wrocic jak zaimplementuje UpdateBoardState

            GwentAction gwentAction = new()
            {
                Id=game.GetNextActionId(),
                ActionType=GwentActionType.CardPlayed,
                Issuer=laneClickedDto.Identity,
                CardPlayed=boardCard,
                CardsAffected=new(),
                AbilitiyUsed=boardCard.Abilities
            };

            await _gameRepository.UpdateGame(game);

            //tutaj jeszcze przychodzi sprawdzanie czy to spy, medyk, scorch itp itd. Cholibka rozległy ten projekt. Fun
            return true;
        }

        public async Task UpdateBoardState(string code)
        {
            Game game = await _gameRepository.GetGameByCode(code);

            var startState = game.CardsOnBoard.Select(x => (x.PrimaryId, x.CurrentStrength));

            //tutaj jeszcze sprawdzic czy w pogodowych jesst scorch (efekt przypisany do karty bede sprawdzal w LaneClicked)
            //trzeba przemyslec podejscie czy tu czy w LaneWeatherClicked. Jeszcze nie wiem
            //if (game.CardsOnBoard.Any(x => x.CardId == 11))
            //{
            //    var nonHeroes = game.CardsOnBoard.Where(x => !x.Abilities.HasFlag(Abilities.Hero));
            //    if (nonHeroes.Any())
            //    {
            //        int maxCurrentSstrength = nonHeroes.Max(x => x.CurrentStrength);
            //        var strongestCards = nonHeroes.Where(x => x.CurrentStrength == maxCurrentSstrength);
            //    }
            //}
                    

            //wyczyszczenie efektow z poprzedniej tury
            foreach(var card in game.CardsOnBoard)
                card.CurrentStrength=card.Strength;


            //pogoda: sprawdzenie czy wystawione slonce, jesli tak to usuwam pogode, jak nie to nakladam efekt
            if (game.CardsOnBoard.Any(x => x.Abilities.HasFlag(Abilities.Clear)))
                game.CardsOnBoard.RemoveAll(x => (x.Abilities & (Abilities.Frost | Abilities.Fog | Abilities.Rain)) != 0);
            else
            {
                ApplyWeather(game.CardsOnBoard, Abilities.Frost, TroopPlacement.Melee);
                ApplyWeather(game.CardsOnBoard, Abilities.Fog, TroopPlacement.Range);
                ApplyWeather(game.CardsOnBoard, Abilities.Rain, TroopPlacement.Siege);
            }
                
            //podanie rączki

            //plusik

            //rog dowodcy
            var playerOneCards = game.CardsOnBoard.Where(x => x.Owner == PlayerIdentity.PlayerOne);
            var playerTwoCards = game.CardsOnBoard.Where(x => x.Owner == PlayerIdentity.PlayerTwo);

            ApplyHorn(playerOneCards, TroopPlacement.Melee);
            ApplyHorn(playerOneCards, TroopPlacement.Range);
            ApplyHorn(playerOneCards, TroopPlacement.Siege);
            ApplyHorn(playerTwoCards, TroopPlacement.Melee);
            ApplyHorn(playerTwoCards, TroopPlacement.Range);
            ApplyHorn(playerTwoCards, TroopPlacement.Siege);

        }

        private void ApplyWeather(List<GwentBoardCard> cards, Abilities ability, TroopPlacement placement)
        {
            if (cards.Any(x => (x.Abilities.HasFlag(ability))))
                foreach (var card in cards.Where(x => x.Placement == placement && !x.Abilities.HasFlag(Abilities.Hero)))
                    card.CurrentStrength = 1;
        }

        private void ApplyHorn(IEnumerable<GwentBoardCard> cards, TroopPlacement placement)
        {
            if (cards.Any(x => x.CardId == 6 && x.Placement == placement))
                foreach (var card in cards.Where(x => x.Placement == placement && !x.Abilities.HasFlag(Abilities.Hero)))
                    card.CurrentStrength *= 2;
        }
    }
}
