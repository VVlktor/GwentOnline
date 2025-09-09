namespace GwentApi.Classes
{
    public class PlayerInfo
    {
        public CardFaction Faction { get; set; }
        public List<GwentCard> Cards { get; set; }//do zapamietania: tutaj zostaje GwentCard, zmienia sie na GwentBoardCard dopiero przy wystawieniu
        public GwentCard LeaderCard { get; set; }
        public byte CardsSwapped { get; set; }
    }
}
