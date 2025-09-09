namespace GwentApi.Classes.Dtos
{
    public class GameStatusDto
    {
        public List<GwentCard> CardsInHand { get; set; }
        public List<GwentBoardCard> CardsOnBoard {  get; set; }
        public PlayerIdentity Turn { get; set; }
        public List<GwentAction> Actions { get; set; }
        public int EnemyCardsCount { get; set; }
        public int PlayerCardsCount {  get; set; }
        
    }
}
