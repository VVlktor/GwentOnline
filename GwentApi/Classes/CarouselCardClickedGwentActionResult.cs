namespace GwentApi.Classes
{
    public class CarouselCardClickedGwentActionResult : BaseGwentActionResult
    {
        public List<GwentBoardCard> PlayedCards { get; set; }
        public List<GwentBoardCard> KilledCards { get; set; }
    }
}
