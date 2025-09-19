namespace GwentApi.Classes
{
    public class LaneClickedGwentActionResult
    {
        public GwentActionType ActionType { get; set; }
        public List<GwentBoardCard> PlayedCards { get; set; }
        public List<GwentBoardCard> KilledCards { get; set; }

    }
}
