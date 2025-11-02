namespace GwentApi.Classes
{
    public class LaneClickedGwentActionResult : BaseGwentActionResult
    {
        public List<GwentBoardCard> PlayedCards { get; set; }
        public List<GwentBoardCard> KilledCards { get; set; }
    }
}
