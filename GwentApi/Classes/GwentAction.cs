namespace GwentApi.Classes
{
    public class GwentAction
    {
        public int Id { get; set; }
        public GwentActionType ActionType { get; set; }
        public PlayerIdentity Issuer { get; set; }
        public List<GwentBoardCard> CardsPlayed { get; set; }
        public List<GwentBoardCard> CardsOnBoard { get; set; }
        public List<GwentBoardCard> CardsKilled { get; set; }
        public Abilities AbilitiyUsed { get; set; }
    }
}
