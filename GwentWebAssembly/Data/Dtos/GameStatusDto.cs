namespace GwentWebAssembly.Data.Dtos
{
    public class GameStatusDto
    {
        public List<GwentCard> CardsInHand { get; set; }
        public List<GwentBoardCard> CardsOnBoard { get; set; }
        public PlayerIdentity Turn { get; set; }
        public PlayerIdentity NextTurn { get; set; }
        public GwentAction Action { get; set; }
        public int EnemyCardsCount { get; set; }
        public int PlayerCardsCount { get; set; }
    }
}
