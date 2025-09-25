namespace GwentApi.Classes
{
    public class CardClickedGwentActionResult
    {
        public GwentActionType ActionType { get; set; }
        public GwentBoardCard PlayedCard { get; set; }
        public GwentBoardCard SwappedCard { get; set; }
    }
}
