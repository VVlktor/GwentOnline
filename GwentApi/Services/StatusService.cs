using GwentApi.Classes;
using GwentApi.Classes.Dtos;
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
                LeaderUsed = leaderUsed
                //AbilitiyUsed = playedCards[0].Abilities//potencjalnie ability do wywalenia
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
            if (game.CardsOnBoard.Any(x => x.Abilities.HasFlag(Abilities.Clear)))
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

            await _gameRepository.UpdateGame(game);
            if (!game.CardsOnBoard.Any(x => x.CardId == 6)) return;

            //string blad = "";
            //foreach (var x in game.CardsOnBoard)
            //    blad+=$"{x.Name} - {x.CurrentStrength}\n";
            //throw new Exception($"{blad}");
        }

        private void ApplyBond(IEnumerable<GwentBoardCard> cards, TroopPlacement placement)
        {
            var bondCards = cards.Where(x => x.Abilities.HasFlag(Abilities.Bond) && x.Placement == placement)
                                 .GroupBy(x => x.Name)
                                 .Where(g => g.Count() > 1);
            foreach (var group in bondCards)
            {
                int multiplier = group.Count();
                foreach (var card in group)
                    if (!card.Abilities.HasFlag(Abilities.Hero))
                        card.CurrentStrength *= multiplier;
            }
        }

        private void ApplyMorale(IEnumerable<GwentBoardCard> cards, TroopPlacement placement)
        {
            foreach(var moraleCard in cards.Where(x => x.Abilities.HasFlag(Abilities.Morale) && x.Placement == placement))
                foreach(var card in cards.Where(x=>x.Placement == placement && !x.Abilities.HasFlag(Abilities.Hero) && x.PrimaryId!=moraleCard.PrimaryId))
                    card.CurrentStrength += 1;
        }

        private void ApplyWeather(IEnumerable<GwentBoardCard> cards, Abilities ability, TroopPlacement placement)
        {
            if (cards.Any(x => (x.Abilities.HasFlag(ability))))
                foreach (var card in cards.Where(x => x.Placement == placement && !x.Abilities.HasFlag(Abilities.Hero)))
                    if(card.Strength!=0)
                        card.CurrentStrength = 1;
        }

        private void ApplyHorn(IEnumerable<GwentBoardCard> cards, TroopPlacement placement)
        {
            if (cards.Any(x => x.CardId == 6 && x.Placement == placement))
                foreach (var card in cards.Where(x => x.Placement == placement && !x.Abilities.HasFlag(Abilities.Hero)))
                    card.CurrentStrength *= 2;
            else
            {
                int hornCount = cards.Count(x => x.Placement==placement && x.Abilities.HasFlag(Abilities.Horn));
                if (hornCount > 1)
                {
                    foreach (var card in cards.Where(x => x.Placement == placement && !x.Abilities.HasFlag(Abilities.Hero)))
                        card.CurrentStrength *= 2;
                }
                else if (hornCount == 1)
                {
                    foreach (var card in cards.Where(x => x.Placement == placement && !x.Abilities.HasFlag(Abilities.Hero) && !x.Abilities.HasFlag(Abilities.Horn)))
                        card.CurrentStrength *= 2;
                }
            }
        }

        public async Task<TurnStatus> UpdateTurn(string code)
        {
            Game game = await _gameRepository.GetGameByCode(code);

            if (game.HasPassed.PlayerOne && game.HasPassed.PlayerTwo)
                return new()
                {
                    EndRound = true,
                    Turn = game.Turn
                };

            PlayerIdentity lastTurn = game.Actions.Last().Issuer;

            bool hasEnemyPassed = lastTurn == PlayerIdentity.PlayerOne ? game.HasPassed.PlayerTwo : game.HasPassed.PlayerOne;

            bool isMedic = game.Actions.Last().ActionType == GwentActionType.MedicCardPlayed && _gameDataService.GetPlayerSide(game, game.Actions.Last().Issuer).UsedCards.Any(x => !x.Abilities.HasFlag(Abilities.Hero) && x.Placement != TroopPlacement.Weather && x.Placement != TroopPlacement.Special);

            if (!hasEnemyPassed && !isMedic)
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

            if (playerOneScore > playerTwoScore)
            {//nie zapominac o tym ze chyba w nilfgaardzie jak jest remis to wygrywa nilfgaard
                playerTwo.Hp -= 1;
                game.Turn = PlayerIdentity.PlayerOne;
            }
            else if (playerTwoScore > playerOneScore)
            {
                playerOne.Hp -= 1;
                game.Turn = PlayerIdentity.PlayerTwo;
            }
            else//remis
            {
                playerOne.Hp -= 1;
                playerTwo.Hp -= 1;
            }

            game.PlayerOne.UsedCards.AddRange( game.CardsOnBoard.Where(x => x.Owner == PlayerIdentity.PlayerOne) );
            game.PlayerTwo.UsedCards.AddRange( game.CardsOnBoard.Where(x => x.Owner == PlayerIdentity.PlayerTwo) );
            game.CardsOnBoard.Clear();

            await _gameRepository.UpdateGame(game);
        }
    }
}
