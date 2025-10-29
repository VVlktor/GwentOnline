namespace GwentApi.Classes
{
    public class LeaderClickedGwentActionResult
    {
        public GwentActionType ActionType { get; set; }
        public GwentBoardCard PlayedCard { get; set; }
        public List<GwentBoardCard> RemovedCards { get; set; }
    }
}
