namespace GwentApi.Classes
{
    public class GwentAction
    {
        public int Id { get; set; }
        public GwentActionType ActionType { get; set; }
        public PlayerIdentity Issuer { get; set; }
        public GwentCard? CardPlayed { get; set; }
        public List<GwentBoardCard> CardsAffected { get; set; }//jesli np. scorch to bedą dwie akcje - jedna zagranie karty, druga zabicie kart. Podobnie medyk: zagranie 1, uleczenie 2
        public Abilities AbilitiyUsed { get; set; }
    }
}
