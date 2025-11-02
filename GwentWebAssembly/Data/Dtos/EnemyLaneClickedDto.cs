namespace GwentWebAssembly.Data.Dtos
{
    public class EnemyLaneClickedDto : BaseClickedDto
    {
        public TroopPlacement Placement { get; set; }
        public GwentCard Card { get; set; }
    }
}
