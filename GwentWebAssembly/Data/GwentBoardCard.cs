namespace GwentWebAssembly.Data
{
    public class GwentBoardCard : GwentCard
    {
        public int CurrentStrength { get; set; }
        public PlayerIdentity Owner { get; set; }
    }
}
