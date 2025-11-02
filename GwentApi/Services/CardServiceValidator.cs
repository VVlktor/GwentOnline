using GwentApi.Classes;
using GwentApi.Classes.Dtos;
using GwentApi.Services.Interfaces;

namespace GwentApi.Services
{
    public class CardServiceValidator : ICardServiceValidator
    {
        private IGameDataService _gameDataService;

        public CardServiceValidator(IGameDataService gameDataService)
        {
            _gameDataService = gameDataService;
        }

        public bool ValidateLane(Game game, LaneClickedDto laneClickedDto)
        {
            if (game.Turn != laneClickedDto.Identity) return false;

            PlayerSide playerSide = _gameDataService.GetPlayerSide(game, laneClickedDto.Identity);

            if (!playerSide.CardsInHand.Any(x => x.PrimaryId == laneClickedDto.Card.PrimaryId)) return false;

            GwentCard card = playerSide.CardsInHand.First(x => x.PrimaryId == laneClickedDto.Card.PrimaryId);

            if (card.Abilities.HasFlag(Abilities.Spy) ||
                card.Placement == TroopPlacement.Weather ||
                card.Placement == TroopPlacement.Special) return false;

            bool canBePlaced = card.Placement == laneClickedDto.Placement
                                                || (card.Placement == TroopPlacement.Agile && (laneClickedDto.Placement == TroopPlacement.Melee
                                                || laneClickedDto.Placement == TroopPlacement.Range));

            return canBePlaced;
        }

        public bool ValidateHorn(Game game, HornClickedDto hornClickedDto)
        {
            PlayerSide playerSide = _gameDataService.GetPlayerSide(game, hornClickedDto.Identity);

            if (game.Turn != hornClickedDto.Identity) return false;
            if (game.CardsOnBoard.Any(x => x.CardId == 6 && x.Placement == hornClickedDto.Placement)) return false;
            if (!playerSide.CardsInHand.Any(x => x.PrimaryId == hornClickedDto.Card.PrimaryId)) return false;

            return true;
        }

        public bool ValidateCard(Game game, CardClickedDto cardClickedDto)
        {
            PlayerSide playerSide = _gameDataService.GetPlayerSide(game, cardClickedDto.Identity);

            if (cardClickedDto.SelectedCard.CardId != 2) return false;
            if (game.Turn != cardClickedDto.Identity) return false;
            if (!playerSide.CardsInHand.Any(x => x.PrimaryId == cardClickedDto.SelectedCard.PrimaryId)) return false;

            GwentCard card = playerSide.CardsInHand.First(x => x.PrimaryId == cardClickedDto.SelectedCard.PrimaryId);

            if (card.CardId != 2) return false;
            if (!game.CardsOnBoard.Any(x => x.PrimaryId == cardClickedDto.ClickedCard.PrimaryId && cardClickedDto.ClickedCard.Owner == cardClickedDto.Identity)) return false;

            GwentBoardCard clickedCard = game.CardsOnBoard.First(x => x.PrimaryId == cardClickedDto.ClickedCard.PrimaryId);

            if (clickedCard.Abilities.HasFlag(Abilities.Hero)) return false;
            if (clickedCard.CardId == 2) return false;

            return true;
        }

        public bool ValidateWeather(Game game, WeatherClickedDto weatherClickedDto)
        {
            PlayerSide playerSide = _gameDataService.GetPlayerSide(game, weatherClickedDto.Identity);

            if (game.Turn != weatherClickedDto.Identity) return false;
            if (!playerSide.CardsInHand.Any(x => x.PrimaryId == weatherClickedDto.Card.PrimaryId)) return false;

            GwentCard card = playerSide.CardsInHand.First(x => x.PrimaryId == weatherClickedDto.Card.PrimaryId);

            if (card.Placement != TroopPlacement.Weather) return false;
            if (game.CardsOnBoard.Any(x => x.CardId == card.CardId)) return false;

            return true;
        }

        public bool ValidateEnemyLane(Game game, EnemyLaneClickedDto enemyLaneClickedDto)
        {
            PlayerSide playerSide = _gameDataService.GetPlayerSide(game, enemyLaneClickedDto.Identity);

            if (game.Turn != enemyLaneClickedDto.Identity) return false;
            if (!playerSide.CardsInHand.Any(x => x.PrimaryId == enemyLaneClickedDto.Card.PrimaryId)) return false;

            GwentCard card = playerSide.CardsInHand.First(x => x.PrimaryId == enemyLaneClickedDto.Card.PrimaryId);

            if (!card.Abilities.HasFlag(Abilities.Spy)) return false;
            if (card.Placement != enemyLaneClickedDto.Placement) return false;

            return true;
        }

        public bool ValidatePass(Game game, PassClickedDto passClickedDto) => game.Turn != passClickedDto.Identity;
     
        public bool ValidateLeader(Game game, LeaderClickedDto leaderClickedDto)
        {
            if (game.Turn != leaderClickedDto.Identity) return false;
            PlayerSide playerSide = _gameDataService.GetPlayerSide(game, leaderClickedDto.Identity);
            if (!playerSide.LeaderCard.LeaderAvailable) return false;
            return true;
        }
    }
}
