namespace GwentShared.Classes.Dtos;

public class LaneClickedDto : BaseClickedDto
{
    public TroopPlacement Placement { get; set; }
    public GwentCard Card { get; set; }
}
