using GwentApi.Classes;
using GwentApi.Repository.Interfaces;
using GwentApi.Services.Interfaces;

namespace GwentApi.Services
{
    public class StatusService : IStatusService
    {
        private IGameRepository _gameRepository;

        public StatusService(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public async Task<GwentAction> AddGwentAction(PlayerIdentity identity, string code, GwentActionType actionType, List<GwentBoardCard> playedCards)
        {
            Game game = await _gameRepository.GetGameByCode(code);

            GwentAction gwentAction = new()
            {
                Id = game.GetNextActionId(),
                ActionType = actionType,
                Issuer = identity,
                CardsPlayed = playedCards,
                CardsOnBoard = game.CardsOnBoard,
                AbilitiyUsed = playedCards[0].Abilities//potencjalnie ability do wywalenia
            };

            game.Actions.Add(gwentAction);

            await _gameRepository.UpdateGame(game);

            return gwentAction;
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

            await _gameRepository.UpdateGame(game);
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
            else
            {
                int hornCount = cards.Count(x => x.Abilities.HasFlag(Abilities.Horn));
                if (hornCount > 1)
                {
                    foreach (var card in cards.Where(x => x.Placement == placement && !x.Abilities.HasFlag(Abilities.Hero)))
                        card.CurrentStrength *= 2;
                }
                else if (hornCount == 1)
                {
                    foreach (var card in cards.Where(x => x.Placement == placement && !x.Abilities.HasFlag(Abilities.Hero) && x.Abilities.HasFlag(Abilities.Horn)))
                        card.CurrentStrength *= 2;
                }
            }
        }
    }
}
