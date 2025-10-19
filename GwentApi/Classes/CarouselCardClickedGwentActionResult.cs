namespace GwentApi.Classes
{
    public class CarouselCardClickedGwentActionResult
    {
        public GwentActionType ActionType { get; set; }
        public List<GwentBoardCard> PlayedCards { get; set; }
        public List<GwentBoardCard> KilledCards { get; set; }
    }
}
