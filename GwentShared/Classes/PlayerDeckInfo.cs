namespace GwentShared.Classes;

public class PlayerDeckInfo
{
    public PlayerDeckInfo()
    {
        
    }

    public PlayerDeckInfo(List<int> cardsId, int leaderCardId, CardFaction faction)
    {
        Faction = faction;
        CardsId = cardsId;
        LeaderCardId = leaderCardId;
    }

    public CardFaction Faction { get; set; }
    public List<int> CardsId { get; set; }
    public int LeaderCardId { get; set; }
}
