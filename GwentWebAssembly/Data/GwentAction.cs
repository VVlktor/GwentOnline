namespace GwentWebAssembly.Data
{
    public class GwentAction
    {
        public int Id { get; set; }
        public GwentActionType ActionType { get; set; }
        public PlayerIdentity Issuer { get; set; }
        public GwentCard? CardPlayed { get; set; }
        public List<GwentBoardCard> CardsAffected { get; set; }
        public Abilities AbilitiyUsed { get; set; }
    }
}
