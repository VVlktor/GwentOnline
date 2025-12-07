namespace GwentShared.Classes.Dtos;

public class GameStatusDto
{
    public List<GwentCard> CardsInHand { get; set; }
    public List<GwentBoardCard> CardsOnBoard { get; set; }
    public List<GwentCard> UsedCards { get; set; }
    public PlayerIdentity Turn { get; set; }
    public GwentAction Action { get; set; }
    public int EnemyCardsCount { get; set; }
    public int EnemyUsedCardsCount { get; set; }
    public int EnemyDeckCount { get; set; }
    public int PlayerDeckCount { get; set; } // karty ktore nie zostaly jeszcze dodane do hand
    public bool PlayerLeaderAvailable { get; set; }
    public bool EnemyLeaderAvailable { get; set; }
    public int PlayerHp { get; set; }
    public int EnemyHp { get; set; }
    public bool PlayerPassed { get; set; }
    public bool EnemyPassed { get; set; }
}
