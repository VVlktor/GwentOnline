using GwentApi.Classes;
using GwentApi.Services.Interfaces;

namespace GwentApi.Services
{
    public class DeckService : IDeckService
    {
        private CardsProvider _cardsProvider;

        public DeckService(CardsProvider cardsProvider)
        {
            _cardsProvider = cardsProvider;
        }

        public ResponseData VerifyDeck(PlayerDeckInfo playerInfo)
        {
            if (playerInfo.CardsId.Count() > 30)
                return new(false, "Card count exceeds 30.");

            List<GwentCard> playerCards = _cardsProvider.GetCards(x => playerInfo.CardsId.Contains(x.PrimaryId)).ToList();
            GwentCard leaderCard = _cardsProvider.GetCardByPrimaryId(playerInfo.LeaderCardId);

            if (leaderCard.Faction != playerInfo.Faction)
                return new(false, "Leader cards is from other faction.");

            if (playerCards.Any(x => x.Faction != playerInfo.Faction && x.Faction != CardFaction.Weather && x.Faction != CardFaction.Special && x.Faction != CardFaction.Neutral))
                return new(false, "Your deck contains cards from other factions.");

            if (playerCards.Count(x => x.Placement == TroopPlacement.Melee || x.Placement == TroopPlacement.Siege || x.Placement == TroopPlacement.Agile || x.Placement == TroopPlacement.Range) < 22)
                return new(false, "Your deck contains less than 22 unit cards.");

            return new(true, "Your deck has been approved. Awaiting for the second player");
        }
    }
}
