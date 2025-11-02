using GwentApi.Classes;
using GwentApi.Extensions;
using GwentApi.Repository.Interfaces;
using GwentApi.Services.Interfaces;

namespace GwentApi.Services
{
    public class StatusService : IStatusService
    {
        private IGameRepository _gameRepository;
        private IGameDataService _gameDataService;

        public StatusService(IGameRepository gameRepository, IGameDataService gameDataService)
        {
            _gameRepository = gameRepository;
            _gameDataService = gameDataService;
        }

        public async Task<GwentAction> AddGwentAction(PlayerIdentity identity, string code, GwentActionType actionType, List<GwentBoardCard> playedCards, List<GwentBoardCard> killedCards, bool leaderUsed = false)
        {
            Game game = await _gameRepository.GetGameByCode(code);

            GwentAction gwentAction = new()
            {
                Id = _gameDataService.GetNextActionId(game),
                ActionType = actionType,
                Issuer = identity,
                CardsPlayed = playedCards,
                CardsOnBoard = game.CardsOnBoard,
                CardsKilled = killedCards,
                LeaderUsed = leaderUsed,
                AbilitiyUsed = playedCards.FirstOrDefault()?.Abilities ?? Abilities.None
            };

            game.Actions.Add(gwentAction);

            await _gameRepository.UpdateGame(game);

            return gwentAction;
        }

        public async Task UpdateBoardState(string code)
        {
            Game game = await _gameRepository.GetGameByCode(code);

            //wyczyszczenie efektow z poprzedniej tury
            foreach (var card in game.CardsOnBoard)
                card.CurrentStrength = card.Strength;

            //pogoda: sprawdzenie czy wystawione slonce, jesli tak to usuwam pogode, jak nie to nakladam efekt
            if(game.CardsOnBoard.Any(x => x.Abilities.HasFlag(Abilities.Clear)))
                game.CardsOnBoard.RemoveAll(x => (x.Abilities & (Abilities.Frost | Abilities.Fog | Abilities.Rain)) != 0);
            else
            {
                ApplyWeather(game.CardsOnBoard, Abilities.Frost, TroopPlacement.Melee);
                ApplyWeather(game.CardsOnBoard, Abilities.Fog, TroopPlacement.Range);
                ApplyWeather(game.CardsOnBoard, Abilities.Rain, TroopPlacement.Siege);
            }

            var playerOneCards = game.CardsOnBoard.Where(x => x.Owner == PlayerIdentity.PlayerOne);
            var playerTwoCards = game.CardsOnBoard.Where(x => x.Owner == PlayerIdentity.PlayerTwo);

            List<TroopPlacement> placements = [TroopPlacement.Melee, TroopPlacement.Range, TroopPlacement.Siege];

            foreach(var placement in placements)
            {
                //podanie rączki(tight bond)
                ApplyBond(playerOneCards, placement);
                ApplyBond(playerTwoCards, placement);
                //plusik (morale)
                ApplyMorale(playerOneCards, placement);
                ApplyMorale(playerTwoCards, placement);
                //rog dowodcy(horn)
                ApplyHorn(playerOneCards, placement);
                ApplyHorn(playerTwoCards, placement);
            }

            ApplyMonstersLeader(game.CardsOnBoard, game.PlayerOne.LeaderCard, game.PlayerTwo.LeaderCard);

            await _gameRepository.UpdateGame(game);
            if(!game.CardsOnBoard.Any(x => x.CardId == 6)) return;
        }

        private void ApplyMonstersLeader(IEnumerable<GwentBoardCard> cards, GwentLeaderCard leaderOne, GwentLeaderCard leaderTwo)
        {
            if((leaderOne.CardId == 98 && leaderOne.LeaderActive) || (leaderTwo.CardId == 98 && leaderTwo.LeaderActive))
                foreach(var spyCard in cards.Where(x => x.Abilities.HasFlag(Abilities.Spy) && !x.Abilities.HasFlag(Abilities.Hero)))
                    spyCard.CurrentStrength *= 2;
        }

        private void ApplyBond(IEnumerable<GwentBoardCard> cards, TroopPlacement placement)
        {
            var bondCards = cards.Where(x => x.Abilities.HasFlag(Abilities.Bond) && x.Placement == placement)
                                 .GroupBy(x => x.Name)
                                 .Where(g => g.Count() > 1);
            foreach(var group in bondCards)
            {
                int multiplier = group.Count();
                foreach(var card in group)
                    if(!card.Abilities.HasFlag(Abilities.Hero))
                        card.CurrentStrength *= multiplier;
            }
        }

        private void ApplyMorale(IEnumerable<GwentBoardCard> cards, TroopPlacement placement)
        {
            foreach(var moraleCard in cards.Where(x => x.Abilities.HasFlag(Abilities.Morale) && x.Placement == placement))
                foreach(var card in cards.Where(x => x.Placement == placement && !x.Abilities.HasFlag(Abilities.Hero) && x.PrimaryId != moraleCard.PrimaryId))
                    card.CurrentStrength += 1;
        }

        private void ApplyWeather(IEnumerable<GwentBoardCard> cards, Abilities ability, TroopPlacement placement)
        {
            if(cards.Any(x => (x.Abilities.HasFlag(ability))))
                foreach(var card in cards.Where(x => x.Placement == placement && !x.Abilities.HasFlag(Abilities.Hero)))
                    if(card.Strength != 0)
                        card.CurrentStrength = 1;
        }

        private void ApplyHorn(IEnumerable<GwentBoardCard> cards, TroopPlacement placement)
        {
            if(cards.Any(x => x.CardId == 6 && x.Placement == placement))
                foreach(var card in cards.Where(x => x.Placement == placement && !x.Abilities.HasFlag(Abilities.Hero)))
                    card.CurrentStrength *= 2;
            else
            {
                int hornCount = cards.Count(x => x.Placement == placement && x.Abilities.HasFlag(Abilities.Horn));
                if(hornCount > 1)
                {
                    foreach (var card in cards.Where(x => x.Placement == placement && !x.Abilities.HasFlag(Abilities.Hero)))
                        card.CurrentStrength *= 2;
                }
                else if(hornCount == 1)
                {
                    foreach (var card in cards.Where(x => x.Placement == placement && !x.Abilities.HasFlag(Abilities.Hero) && !x.Abilities.HasFlag(Abilities.Horn)))
                        card.CurrentStrength *= 2;
                }
            }
        }

        public async Task<TurnStatus> UpdateTurn(string code)
        {
            Game game = await _gameRepository.GetGameByCode(code);

            if(game.HasPassed.PlayerOne && game.HasPassed.PlayerTwo)
                return new()
                {
                    EndRound = true,
                    Turn = game.Turn
                };

            PlayerIdentity lastTurn = game.Actions.Last().Issuer;

            bool hasEnemyPassed = lastTurn == PlayerIdentity.PlayerOne ? game.HasPassed.PlayerTwo : game.HasPassed.PlayerOne;

            bool isMedic = game.Actions.Last().ActionType == GwentActionType.MedicCardPlayed && _gameDataService.GetPlayerSide(game, game.Actions.Last().Issuer).UsedCards.Any(x => !x.Abilities.HasFlag(Abilities.Hero) && x.Placement != TroopPlacement.Weather && x.Placement != TroopPlacement.Special);

            if(!hasEnemyPassed && !isMedic)
                game.Turn = lastTurn.GetEnemy();

            await _gameRepository.UpdateGame(game);

            return new()
            {
                EndRound = false,
                Turn = game.Turn
            };
        }

        public async Task EndRound(string code)
        {
            Game game = await _gameRepository.GetGameByCode(code);

            PlayerSide playerOne = game.PlayerOne;
            PlayerSide playerTwo = game.PlayerTwo;

            int playerOneScore = game.CardsOnBoard.Where(x => x.Owner == PlayerIdentity.PlayerOne).Sum(x => x.CurrentStrength);
            int playerTwoScore = game.CardsOnBoard.Where(x => x.Owner == PlayerIdentity.PlayerTwo).Sum(x => x.CurrentStrength);

            game.HasPassed = (false, false);

            Random rnd = new();

            if(playerOneScore > playerTwoScore)
            {
                playerTwo.Hp -= 1;
                game.Turn = PlayerIdentity.PlayerOne;
                HandleNorthern(playerOne, rnd);
            }
            else if(playerTwoScore > playerOneScore)
            {
                playerOne.Hp -= 1;
                game.Turn = PlayerIdentity.PlayerTwo;
                HandleNorthern(playerTwo, rnd);
            }
            else//remis
            {
                if (playerOne.Faction != CardFaction.NilfgaardianEmpire)
                    playerOne.Hp -= 1;
                if (playerTwo.Faction != CardFaction.NilfgaardianEmpire)
                    playerTwo.Hp -= 1;
            }

            game.PlayerOne.UsedCards.AddRange(game.CardsOnBoard.Where(x => x.Owner == PlayerIdentity.PlayerOne));
            game.PlayerTwo.UsedCards.AddRange(game.CardsOnBoard.Where(x => x.Owner == PlayerIdentity.PlayerTwo));

            GwentBoardCard cardOne = await HandleMonsters(game, playerOne, PlayerIdentity.PlayerOne, rnd);
            GwentBoardCard cardTwo = await HandleMonsters(game, playerTwo, PlayerIdentity.PlayerTwo, rnd);

            game.CardsOnBoard.Clear();

            if(cardOne is not null)
                game.CardsOnBoard.Add(cardOne);
            if(cardTwo is not null)
                game.CardsOnBoard.Add(cardTwo);

            playerOne.LeaderCard.LeaderActive = false;//moze bedzie jakis ktory trwa cala gre, wtedy sie bede martwil
            playerTwo.LeaderCard.LeaderActive = false;

            await _gameRepository.UpdateGame(game);
        }

        private void HandleNorthern(PlayerSide playerSide, Random rnd)
        {
            if(playerSide.Deck.Count == 0) return;

            GwentCard additionalCard = playerSide.Deck[rnd.Next(playerSide.Deck.Count)];

            playerSide.Deck.Remove(additionalCard);
            playerSide.CardsInHand.Add(additionalCard);
        }

        private async Task<GwentBoardCard> HandleMonsters(Game game, PlayerSide playerSide, PlayerIdentity identity, Random rnd)
        {
            if(playerSide.Faction != CardFaction.Monsters) return null;

            var playersCards = game.CardsOnBoard.Where(x => x.Owner == identity && (x.Placement == TroopPlacement.Melee || x.Placement == TroopPlacement.Siege || x.Placement == TroopPlacement.Range) && x.CardId != 6 && x.CardId != 2).ToList();

            if(playersCards.Count == 0) return null;

            GwentBoardCard randomCard = playersCards[rnd.Next(playersCards.Count)];

            playerSide.UsedCards.Remove(randomCard);

            return randomCard;
        }
    }
}
