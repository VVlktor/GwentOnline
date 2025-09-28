namespace GwentWebAssembly.Data.Dtos
{
    public class EnemyLaneClickedDto
    {
        public TroopPlacement Placement { get; set; }
        public PlayerIdentity Identity { get; set; }
        public string Code { get; set; }
        public GwentCard Card { get; set; }
    }
}
