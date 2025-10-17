namespace GwentApi.Classes
{
    public class PlayerDeckInfo
    {
        public CardFaction Faction { get; set; }
        public List<int> CardsId { get; set; }
        public int LeaderCardId { get; set; }
    }
}
