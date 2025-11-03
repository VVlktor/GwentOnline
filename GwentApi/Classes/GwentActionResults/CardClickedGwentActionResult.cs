namespace GwentApi.Classes.GwentActionResults
{
    public class CardClickedGwentActionResult : BaseGwentActionResult
    {
        public GwentBoardCard PlayedCard { get; set; }
        public GwentBoardCard SwappedCard { get; set; }
    }
}
