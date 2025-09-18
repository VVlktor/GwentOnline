namespace GwentWebAssembly.Data.Dtos
{
    public class HornClickedDto
    {
        public PlayerIdentity Identity { get; set; }
        public string Code { get; set; }
        public TroopPlacement Placement { get; set; }
        public GwentCard Card { get; set; }
    }
}
