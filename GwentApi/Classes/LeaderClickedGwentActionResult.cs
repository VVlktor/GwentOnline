namespace GwentApi.Classes
{
    public class LeaderClickedGwentActionResult : BaseGwentActionResult
    {
        public GwentBoardCard PlayedCard { get; set; }
        public List<GwentBoardCard> RemovedCards { get; set; }
    }
}
