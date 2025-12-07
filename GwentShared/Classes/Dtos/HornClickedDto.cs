namespace GwentShared.Classes.Dtos;

public class HornClickedDto : BaseClickedDto
{
    public TroopPlacement Placement { get; set; }
    public GwentCard Card { get; set; }
}
