using GwentShared.Classes;
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
            List<GwentCard> playerCards = _cardsProvider.GetCards(x => playerInfo.CardsId.Contains(x.PrimaryId)).ToList();
            GwentCard leaderCard = _cardsProvider.GetCardByPrimaryId(playerInfo.LeaderCardId);

            if (leaderCard.Faction != playerInfo.Faction)
                return new(false, "Leader cards is from other faction.");

            if (playerCards.Any(x => x.Faction != playerInfo.Faction && x.Faction != CardFaction.Weather && x.Faction != CardFaction.Special && x.Faction != CardFaction.Neutral))
                return new(false, "Your deck contains cards from other factions.");

            if (playerCards.Count(x => x.Placement is TroopPlacement.Melee or TroopPlacement.Siege or TroopPlacement.Agile or TroopPlacement.Range) < 22)
                return new(false, "Your deck contains less than 22 unit cards.");

            return new(true, "Your deck has been approved. Awaiting for the second player");
        }
    }
}