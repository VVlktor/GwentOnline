namespace GwentWebAssembly.Data
{
    public class PlayerInfo
    {
        public PlayerInfo(CardFaction faction, List<GwentCard> cards, GwentCard leaderCard)
        {
            Faction = faction;
            Cards = cards;
            LeaderCard = leaderCard;
            CardsSwapped = 0;
        }

        public CardFaction Faction { get; set; }
        public List<GwentCard> Cards { get; set; }
        public GwentCard LeaderCard { get; set; }
        public byte CardsSwapped { get; set; }
    }
}
