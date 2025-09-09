namespace GwentApi.Classes
{
    public class GwentBoardCard : GwentCard
    {
        public int CurrentStrength { get; set; }
        public PlayerIdentity Owner { get; set; }
    }
}
