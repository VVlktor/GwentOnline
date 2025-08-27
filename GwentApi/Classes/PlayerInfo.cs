namespace GwentApi.Classes
{
    public class PlayerInfo
    {
        public CardFaction Faction { get; set; }
        public List<GwentCard> Cards { get; set; }
        public GwentCard LeaderCard { get; set; }
    }
}
