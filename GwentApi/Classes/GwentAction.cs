namespace GwentApi.Classes
{
    public class GwentAction
    {
        public int Id { get; set; }
        public GwentActionType ActionType { get; set; }
        public PlayerIdentity Issuer { get; set; }
        public GwentBoardCard? CardPlayed { get; set; }
        public List<GwentBoardCard> CardsAffected { get; set; }
        public Abilities AbilitiyUsed { get; set; }
    }
}
