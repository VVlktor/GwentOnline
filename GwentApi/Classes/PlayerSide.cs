using GwentShared.Classes;

namespace GwentApi.Classes
{
    public class PlayerSide
    {
        public List<GwentCard> Deck = new();
        public List<GwentCard> CardsInHand = new();
        public List<GwentCard> UsedCards = new();
        public CardFaction Faction { get; set; }
        public GwentLeaderCard LeaderCard { get; set; }
        public int Hp { get; set; }
    }
}
