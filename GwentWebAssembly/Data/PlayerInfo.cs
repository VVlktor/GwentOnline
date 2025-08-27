namespace GwentWebAssembly.Data
{
    public class PlayerInfo
    {
        public PlayerInfo(CardFaction faction, List<GwentCard> gwentCards, GwentCard leaderCard)
        {
            Faction = faction;
            Cards = gwentCards;
            LeaderCard = leaderCard;
        }

        public CardFaction Faction { get; set; }
        public List<GwentCard> Cards { get; set; }
        public GwentCard LeaderCard { get; set; }
    }
}
