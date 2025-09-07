namespace GwentApi.Classes
{
    public class PlayerSide
    {
        public List<GwentCard> Deck = new();
        public List<GwentCard> CardsInHand = new();
        public List<GwentCard> UsedCards = new();
        public CardFaction Faction { get; set; }
        public GwentCard LeaderCard { get; set; }
    }
}
