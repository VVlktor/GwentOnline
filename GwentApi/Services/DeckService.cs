using GwentApi.Classes;
using GwentApi.Services.Interfaces;

namespace GwentApi.Services
{
    public class DeckService : IDeckService
    {
        public ResponseData VerifyDeck(PlayerInfo playerInfo)
        {
            if (playerInfo.Cards.Count() > 30)
                return new(false, "Card count exceeds 30.");

            if (playerInfo.Cards.Any(x => x.Faction != playerInfo.Faction && x.Faction != CardFaction.Weather && x.Faction != CardFaction.Special && x.Faction != CardFaction.Neutral))
                return new(false, "Your deck contains cards from other factions.");

            if (playerInfo.Cards.Count(x => x.Placement == TroopPlacement.Melee || x.Placement == TroopPlacement.Siege || x.Placement == TroopPlacement.Agile || x.Placement == TroopPlacement.Range) < 22)
                return new(false, "Your deck contains less than 22 unit cards.");

            return new(true, "Your deck has been approved. Awaiting for the second player");
        }
    }
}
