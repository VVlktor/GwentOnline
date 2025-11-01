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

        public bool ValidateLaneClicked(Game game, PlayerIdentity identity, LaneClickedDto laneClickedDto)
        {
            if (game.Turn != identity) return false;

            PlayerSide playerSide = _gameDataService.GetPlayerSide(game, identity);

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
    }
}
