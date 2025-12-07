using GwentShared.Classes;

namespace GwentApi.Classes.GwentActionResults
{
    public class WeatherClickedGwentActionResult : BaseGwentActionResult
    {
        public GwentBoardCard PlayedCard { get; set; }
        public List<GwentBoardCard> RemovedCards { get; set; }
    }
}
